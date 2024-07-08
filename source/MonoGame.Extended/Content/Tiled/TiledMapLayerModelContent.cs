// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapLayerModelContent
{
    private readonly List<VertexPositionTexture> _vertices;
    private readonly List<ushort> _indices;

    public string LayerName { get; }
    public ReadOnlyCollection<VertexPositionTexture> Vertices { get; }
    public ReadOnlyCollection<ushort> Indices { get; }
    public SizeF ImageSize { get; }
    public string TextureAssetName { get; }

    public TiledMapLayerModelContent(string layerName, TiledMapImageContent image)
    {
        LayerName = layerName;
        _vertices = new List<VertexPositionTexture>();
        Vertices = new ReadOnlyCollection<VertexPositionTexture>(_vertices);
        _indices = new List<ushort>();
        Indices = new ReadOnlyCollection<ushort>(_indices);
        ImageSize = new SizeF(image.Width, image.Height);
        TextureAssetName = Path.ChangeExtension(image.Source, null);
    }

    public TiledMapLayerModelContent(string layerName, TiledMapTilesetContent tileset)
        : this(layerName, tileset.Image)
    {
    }

    public void AddTileVertices(Vector2 position, Rectangle? sourceRectangle = null, TiledMapTileFlipFlags flags = TiledMapTileFlipFlags.None)
    {
        float texelLeft, texelTop, texelRight, texelBottom;
        var sourceRectangle1 = sourceRectangle ?? new Rectangle(0, 0, (int)ImageSize.Width, (int)ImageSize.Height);

        if (sourceRectangle.HasValue)
        {
            var reciprocalWidth = 1f / ImageSize.Width;
            var reciprocalHeight = 1f / ImageSize.Height;
            texelLeft = sourceRectangle1.X * reciprocalWidth;
            texelTop = sourceRectangle1.Y * reciprocalHeight;
            texelRight = (sourceRectangle1.X + sourceRectangle1.Width) * reciprocalWidth;
            texelBottom = (sourceRectangle1.Y + sourceRectangle1.Height) * reciprocalHeight;
        }
        else
        {
            texelLeft = 0;
            texelTop = 0;
            texelBottom = 1;
            texelRight = 1;
        }

        VertexPositionTexture vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight;

        vertexTopLeft.Position = new Vector3(position, 0);
        vertexTopRight.Position = new Vector3(position + new Vector2(sourceRectangle1.Width, 0), 0);
        vertexBottomLeft.Position = new Vector3(position + new Vector2(0, sourceRectangle1.Height), 0);
        vertexBottomRight.Position =
            new Vector3(position + new Vector2(sourceRectangle1.Width, sourceRectangle1.Height), 0);

        vertexTopLeft.TextureCoordinate.Y = texelTop;
        vertexTopLeft.TextureCoordinate.X = texelLeft;

        vertexTopRight.TextureCoordinate.Y = texelTop;
        vertexTopRight.TextureCoordinate.X = texelRight;

        vertexBottomLeft.TextureCoordinate.Y = texelBottom;
        vertexBottomLeft.TextureCoordinate.X = texelLeft;

        vertexBottomRight.TextureCoordinate.Y = texelBottom;
        vertexBottomRight.TextureCoordinate.X = texelRight;

        var flipDiagonally = (flags & TiledMapTileFlipFlags.FlipDiagonally) != 0;
        var flipHorizontally = (flags & TiledMapTileFlipFlags.FlipHorizontally) != 0;
        var flipVertically = (flags & TiledMapTileFlipFlags.FlipVertically) != 0;

        if (flipDiagonally)
        {
            FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.X, ref vertexBottomLeft.TextureCoordinate.X);
            FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.Y, ref vertexBottomLeft.TextureCoordinate.Y);
        }

        if (flipHorizontally)
        {
            if (flipDiagonally)
            {
                FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.Y, ref vertexTopRight.TextureCoordinate.Y);
                FloatHelper.Swap(ref vertexBottomLeft.TextureCoordinate.Y, ref vertexBottomRight.TextureCoordinate.Y);
            }
            else
            {
                FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.X, ref vertexTopRight.TextureCoordinate.X);
                FloatHelper.Swap(ref vertexBottomLeft.TextureCoordinate.X, ref vertexBottomRight.TextureCoordinate.X);
            }
        }

        if (flipVertically)
            if (flipDiagonally)
            {
                FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.X, ref vertexBottomLeft.TextureCoordinate.X);
                FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.X, ref vertexBottomRight.TextureCoordinate.X);
            }
            else
            {
                FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.Y, ref vertexBottomLeft.TextureCoordinate.Y);
                FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.Y, ref vertexBottomRight.TextureCoordinate.Y);
            }

        _vertices.Add(vertexTopLeft);
        _vertices.Add(vertexTopRight);
        _vertices.Add(vertexBottomLeft);
        _vertices.Add(vertexBottomRight);

        Debug.Assert(Vertices.Count <= TiledMapHelper.MaximumVerticesPerModel);
    }

    public void AddTileIndices()
    {
        var indexOffset = Vertices.Count;

        Debug.Assert(3 + indexOffset <= TiledMapHelper.MaximumVerticesPerModel);

        _indices.Add((ushort)(0 + indexOffset));
        _indices.Add((ushort)(1 + indexOffset));
        _indices.Add((ushort)(2 + indexOffset));
        _indices.Add((ushort)(1 + indexOffset));
        _indices.Add((ushort)(3 + indexOffset));
        _indices.Add((ushort)(2 + indexOffset));

        Debug.Assert(Indices.Count <= TiledMapHelper.MaximumIndicesPerModel);
    }
}
