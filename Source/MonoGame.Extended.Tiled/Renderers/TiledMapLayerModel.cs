using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    public abstract class TiledMapLayerModel : IDisposable
    {
        protected TiledMapLayerModel(GraphicsDevice graphicsDevice, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices)
        {
            Texture = texture;

            // ReSharper disable once VirtualMemberCallInConstructor
            VertexBuffer = CreateVertexBuffer(graphicsDevice, vertices.Length);
            VertexBuffer.SetData(vertices, 0, vertices.Length);

            // ReSharper disable once VirtualMemberCallInConstructor
            IndexBuffer = CreateIndexBuffer(graphicsDevice, indices.Length);
            IndexBuffer.SetData(indices, 0, indices.Length);

            TriangleCount = indices.Length / 3;
        }

        public void Dispose()
        {
            IndexBuffer.Dispose();
            VertexBuffer.Dispose();
        }

        public Texture2D Texture { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public int TriangleCount { get; }

        protected abstract VertexBuffer CreateVertexBuffer(GraphicsDevice graphicsDevice, int vertexCount);
        protected abstract IndexBuffer CreateIndexBuffer(GraphicsDevice graphicsDevice, int indexCount);

    }
}