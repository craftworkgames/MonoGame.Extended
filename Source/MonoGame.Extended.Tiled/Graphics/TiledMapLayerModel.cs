using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Graphics
{
    public class TiledMapLayerModel : IDisposable
    {
        public string LayerName { get; }
        public Texture2D Texture { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public int TrianglesCount { get; }

        internal VertexPositionTexture[] Vertices;

        internal TiledMapLayerModel(ContentReader reader, bool isDynamicData = false)
        {
            var graphicsDevice = reader.GetGraphicsDevice();

            LayerName = reader.ReadString();
            var textureAssetName = reader.GetRelativeAssetName(reader.ReadString());
            Texture = reader.ContentManager.Load<Texture2D>(textureAssetName);

            var vertexCount = reader.ReadInt32();
            var vertices = new VertexPositionTexture[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var textureCoordinateX = reader.ReadSingle();
                var textureCoordinateY = reader.ReadSingle();
                vertices[i] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(textureCoordinateX, textureCoordinateY));
            }

            if (isDynamicData)
                Vertices = vertices;
            
            VertexBuffer = isDynamicData
                ? new DynamicVertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly)
                : new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices, 0, vertexCount);

            var indexCount = reader.ReadInt32();
            var indices = new ushort[indexCount];
            for (var i = 0; i < indexCount; i++)
            {
                indices[i] = reader.ReadUInt16();
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