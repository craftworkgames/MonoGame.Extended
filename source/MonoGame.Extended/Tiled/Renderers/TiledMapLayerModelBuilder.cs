using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public abstract class TiledMapLayerModelBuilder<T>
    {
        protected TiledMapLayerModelBuilder()
        {
            Indices = new List<ushort>();
            Vertices = new List<VertexPositionTexture>();
        }

        public List<ushort> Indices { get; }
        public List<VertexPositionTexture> Vertices { get; }
        public bool IsFull => Vertices.Count + TiledMapHelper.VerticesPerTile >= TiledMapHelper.MaximumVerticesPerModel;
        public bool IsBuildable => Vertices.Any();

        protected abstract void ClearBuffers();
        protected abstract T CreateModel(GraphicsDevice graphicsDevice, Texture2D texture);

        public T Build(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            var model = CreateModel(graphicsDevice, texture);
            Vertices.Clear();
            Indices.Clear();
            ClearBuffers();
            return model;
        }

        public void AddSprite(Texture2D texture, Vector2 position, Rectangle sourceRectangle, TiledMapTileFlipFlags flipFlags)
        {
            Indices.AddRange(CreateTileIndices(Vertices.Count));
            Debug.Assert(Indices.Count <= TiledMapHelper.MaximumIndicesPerModel);

            Vertices.AddRange(CreateVertices(texture, position, sourceRectangle, flipFlags));
            Debug.Assert(Vertices.Count <= TiledMapHelper.MaximumVerticesPerModel);
        }

        private static IEnumerable<VertexPositionTexture> CreateVertices(Texture2D texture, Vector2 position, Rectangle sourceRectangle, TiledMapTileFlipFlags flags = TiledMapTileFlipFlags.None)
        {
            var reciprocalWidth = 1f / texture.Width;
            var reciprocalHeight = 1f / texture.Height;
            var texelLeft = sourceRectangle.X * reciprocalWidth;
            var texelTop = sourceRectangle.Y * reciprocalHeight;
            var texelRight = (sourceRectangle.X + sourceRectangle.Width) * reciprocalWidth;
            var texelBottom = (sourceRectangle.Y + sourceRectangle.Height) * reciprocalHeight;

            VertexPositionTexture vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight;

            vertexTopLeft.Position = new Vector3(position, 0);
            vertexTopRight.Position = new Vector3(position + new Vector2(sourceRectangle.Width, 0), 0);
            vertexBottomLeft.Position = new Vector3(position + new Vector2(0, sourceRectangle.Height), 0);
            vertexBottomRight.Position = new Vector3(position + new Vector2(sourceRectangle.Width, sourceRectangle.Height), 0);

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
            {
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
            }

            yield return vertexTopLeft;
            yield return vertexTopRight;
            yield return vertexBottomLeft;
            yield return vertexBottomRight;
        }

        private static IEnumerable<ushort> CreateTileIndices(int indexOffset)
        {
            yield return (ushort)(0 + indexOffset);
            yield return (ushort)(1 + indexOffset);
            yield return (ushort)(2 + indexOffset);
            yield return (ushort)(1 + indexOffset);
            yield return (ushort)(3 + indexOffset);
            yield return (ushort)(2 + indexOffset);
        }
    }
}
