using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Renderers
{
    public class GroupRenderDetails
    {
        public GroupRenderDetails(GraphicsDevice gd, IEnumerable<VertexPositionTexture> vertices,
            IEnumerable<ushort> indexes, Texture2D texture)
        {
            VertexBuffer = new VertexBuffer(gd, typeof(VertexPositionTexture), vertices.Count(), BufferUsage.WriteOnly);
            IndexBuffer = new IndexBuffer(gd, typeof(ushort), indexes.Count(), BufferUsage.WriteOnly);
            SetVertices(vertices);
            SetIndexes(indexes);
            Texture = texture;
        }

        public GroupRenderDetails(GraphicsDevice gd, IEnumerable<VertexPositionTexture> vertices,
            IEnumerable<ushort> indexes, Texture2D texture, int tileCount) : this(gd, vertices, indexes, texture)
        {
            TileCount = tileCount;
        }

        public Texture2D Texture { get; set; }
        public int TileCount { get; set; }
        public List<VertexPositionTexture> Vertices { get; private set; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public float Opacity { get; set; }

        public void SetVertices(IEnumerable<VertexPositionTexture> vertices)
        {
            Vertices = new List<VertexPositionTexture>(vertices.Count());
            Vertices.AddRange(vertices);
            VertexBuffer.SetData(vertices.ToArray());
        }

        public void SetIndexes(IEnumerable<ushort> indexes)
        {
            var indexArray = indexes.ToArray();
            IndexBuffer.SetData(indexArray);
        }
    }
}