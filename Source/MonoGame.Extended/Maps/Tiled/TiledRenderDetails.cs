using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledRenderDetails
    {
        public TiledRenderDetails(GraphicsDevice gd, int tileCount, List<VertexPositionTexture> vertices, List<short> indexes)
        {
            TileCount = tileCount;

            VertexBuffer = new VertexBuffer(gd, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices.ToArray());
            IndexBuffer = new IndexBuffer(gd, typeof(short), indexes.Count, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indexes.ToArray());
        }

        public int TileCount { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
    }
}