using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Renderers
{
    internal class DynamicGroupRenderDetails : GroupRenderDetails
    {
        public List<ITiledAnimated> AnimatedTiles;

        public DynamicGroupRenderDetails(GraphicsDevice graphicsDevice, IEnumerable<VertexPositionTexture> vertices,
            IEnumerable<ushort> indexes, Texture2D texture, List<ITiledAnimated> animatedTiles)
            : base(graphicsDevice, vertices, indexes, texture)
        {
            AnimatedTiles = animatedTiles;
            TileCount = animatedTiles.Count;
        }
    }
}