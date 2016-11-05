using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Renderers
{
    public class GroupRenderDetails
    {
        public GroupRenderDetails(Texture2D texture)
        {
            Texture = texture;
        }

        public GroupRenderDetails(Texture2D texture, int tileCount)
        {
            Texture = texture;
            TileCount = tileCount;
        }

        public Texture2D Texture { get; set; }
        public int TileCount { get; set; }
        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }

        public void SetVertices(IEnumerable<VertexPositionTexture> vertices, GraphicsDevice gd)
        {
            VertexPositionTexture[] vertArray = vertices.ToArray();
            VertexBuffer vb = new VertexBuffer(gd, typeof(VertexPositionTexture), vertArray.Length, BufferUsage.WriteOnly);
            vb.SetData(vertArray);
            VertexBuffer = vb;
        }

        public void SetIndexes(IEnumerable<ushort> indexes, GraphicsDevice gd)
        {
            ushort[] indexArray = indexes.ToArray();
            IndexBuffer ib = new IndexBuffer(gd, typeof(ushort), indexArray.Length, BufferUsage.WriteOnly);
            ib.SetData(indexArray);
            IndexBuffer = ib;
        }
    }
}