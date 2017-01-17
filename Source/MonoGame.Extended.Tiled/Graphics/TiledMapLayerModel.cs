#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

#endregion

namespace MonoGame.Extended.Tiled.Graphics
{
    public class TiledMapLayerModel : IDisposable
    {
        public string LayerName { get; }
        public Texture2D Texture { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public int TrianglesCount { get; }

        internal VertexPositionTexture[] Vertices;

        internal TiledMapLayerModel(ContentReader input, bool isDynamicData = false)
        {
            var graphicsDevice = input.GetGraphicsDevice();

            LayerName = input.ReadString();
            var textureAssetName = input.ReadString();
            Texture = input.ContentManager.Load<Texture2D>(textureAssetName);

            var vertexCount = input.ReadInt32();
            var vertices = new VertexPositionTexture[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                var x = input.ReadSingle();
                var y = input.ReadSingle();
                var textureCoordinateX = input.ReadSingle();
                var textureCoordinateY = input.ReadSingle();
                vertices[i] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(textureCoordinateX, textureCoordinateY));
            }

            if (isDynamicData)
                Vertices = vertices;
            
            VertexBuffer = isDynamicData
                ? new DynamicVertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly)
                : new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices, 0, vertexCount);

            var indexCount = input.ReadInt32();
            var indices = new ushort[indexCount];
            for (var i = 0; i < indexCount; i++)
            {
                indices[i] = input.ReadUInt16();
            }

            IndexBuffer = isDynamicData
                ? new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly)
                : new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indices, 0, indexCount);

            TrianglesCount = indexCount / 3;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
                return;
            IndexBuffer.Dispose();
            VertexBuffer.Dispose();
        }
    }
}