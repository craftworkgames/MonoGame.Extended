using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Maps.Renderers {
    class DynamicGroupRenderDetails : GroupRenderDetails
    {
        public List<TiledTile> Tiles;

        public DynamicGroupRenderDetails(GraphicsDevice gd, IEnumerable<VertexPositionTexture> vertices, IEnumerable<ushort> indexes, Texture2D texture, List<TiledTile> tiles) : base(gd, vertices, indexes, texture)
        {
            Tiles = tiles;
            TileCount = tiles.Count;
        }
    }
}
