using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Rendering;

internal static class TilemapRendererShared
{
    internal static LayerModel CreateLayerModel(GraphicsDevice graphicsDevice, VertexPositionColorTexture[] vertices, int[] indices, Texture2D texture)
    {
        VertexBuffer vertexBuffer = new VertexBuffer(
            graphicsDevice,
            typeof(VertexPositionColorTexture),
            vertices.Length,
            BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);

        IndexBuffer indexBuffer;
        if (CanUseSixteenBitIndices(vertices, indices))
        {
            ushort[] shortIndices = new ushort[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                shortIndices[i] = (ushort)indices[i];
            }

            indexBuffer = new IndexBuffer(
                graphicsDevice,
                IndexElementSize.SixteenBits,
                shortIndices.Length,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(shortIndices);
        }
        else
        {
            // 32-bit indices support groups larger than 16,383 tiles (the 16-bit limit).
            indexBuffer = new IndexBuffer(
                graphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                indices.Length,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        return new LayerModel
        {
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,
            Texture = texture,
            PrimitiveCount = indices.Length / 3
        };
    }

    private static bool CanUseSixteenBitIndices(VertexPositionColorTexture[] vertices, int[] indices)
    {
        if (vertices.Length > ushort.MaxValue)
        {
            return false;
        }

        for (int i = 0; i < indices.Length; i++)
        {
            if ((uint)indices[i] > ushort.MaxValue)
            {
                return false;
            }
        }

        return true;
    }

    internal static void AddTileQuad(List<VertexPositionColorTexture> vertices, List<int> indices, Vector2 position, int width, int height, Rectangle sourceRect, TilemapTileFlipFlags flipFlags, Texture2D texture, Color color)
    {
        Vector3 topLeft = new Vector3(position.X, position.Y, 0);
        Vector3 topRight = new Vector3(position.X + width, position.Y, 0);
        Vector3 bottomLeft = new Vector3(position.X, position.Y + height, 0);
        Vector3 bottomRight = new Vector3(position.X + width, position.Y + height, 0);

        Vector2[] uvs = CalculateTextureCoordinates(sourceRect, flipFlags, texture);

        int vertexOffset = vertices.Count;
        vertices.Add(new VertexPositionColorTexture(topLeft, color, uvs[0]));
        vertices.Add(new VertexPositionColorTexture(topRight, color, uvs[1]));
        vertices.Add(new VertexPositionColorTexture(bottomLeft, color, uvs[2]));
        vertices.Add(new VertexPositionColorTexture(bottomRight, color, uvs[3]));

        // Counter-clockwise winding matches MonoGame's default CullCounterClockwiseFace rasterizer state.
        indices.Add(vertexOffset);
        indices.Add(vertexOffset + 1);
        indices.Add(vertexOffset + 2);
        indices.Add(vertexOffset + 1);
        indices.Add(vertexOffset + 3);
        indices.Add(vertexOffset + 2);
    }

    internal static Vector2[] CalculateTextureCoordinates(Rectangle sourceRect, TilemapTileFlipFlags flipFlags, Texture2D texture)
    {
        // Normalize source rectangle to 0-1 UV range.
        // Direct edge-to-edge mapping is correct for PointClamp: pixel centers are at
        // half-integer positions and never coincide with a UV boundary, so no texel inset
        // is needed. An inset would compress n texels into an n-1 texel UV span, causing
        // some screen pixels to sample the wrong texel at non-1:1 display scales.
        float left = sourceRect.Left / (float)texture.Width;
        float right = sourceRect.Right / (float)texture.Width;
        float top = sourceRect.Top / (float)texture.Height;
        float bottom = sourceRect.Bottom / (float)texture.Height;
        Vector2[] destinationUvs = new Vector2[4];

        for (int sourceIndex = 0; sourceIndex < 4; sourceIndex++)
        {
            int x = sourceIndex % 2;
            int y = sourceIndex / 2;
            float u = x == 0 ? left : right;
            float v = y == 0 ? top : bottom;

            // Tiled applies the diagonal flip first, then horizontal and vertical flips.
            if ((flipFlags & TilemapTileFlipFlags.FlipDiagonally) != 0)
            {
                (x, y) = (y, x);
            }

            if ((flipFlags & TilemapTileFlipFlags.FlipHorizontally) != 0)
            {
                x = 1 - x;
            }

            if ((flipFlags & TilemapTileFlipFlags.FlipVertically) != 0)
            {
                y = 1 - y;
            }

            destinationUvs[y * 2 + x] = new Vector2(u, v);
        }

        return destinationUvs;
    }

    internal static SamplerState GetWrapSamplerState(SamplerState samplerState)
    {
        if (samplerState == SamplerState.PointClamp)
        {
            return SamplerState.PointWrap;
        }

        if (samplerState == SamplerState.LinearClamp)
        {
            return SamplerState.LinearWrap;
        }

        // If the caller already configured a wrap state (or any other custom state), use it as-is.
        return samplerState;
    }
}

internal sealed class LayerModel : IDisposable
{
    public VertexBuffer VertexBuffer { get; set; }
    public IndexBuffer IndexBuffer { get; set; }
    public Texture2D Texture { get; set; }
    public int PrimitiveCount { get; set; }
    public Vector2 ParallaxFactor { get; set; } = Vector2.One;

    public void Dispose()
    {
        VertexBuffer?.Dispose();
        IndexBuffer?.Dispose();
    }
}
