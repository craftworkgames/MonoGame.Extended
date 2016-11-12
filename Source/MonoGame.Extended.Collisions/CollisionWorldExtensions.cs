using System.Linq;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Collisions
{
    public static class CollisionWorldExtensions
    {
        public static CollisionGrid CreateGrid(this CollisionWorld world, TiledTileLayer tileLayer)
        {
            var data = tileLayer.Tiles
                .Select(t => t.Id)
                .ToArray();

            return world.CreateGrid(data, tileLayer.Width, tileLayer.Height, tileLayer.TileWidth, tileLayer.TileHeight);
        }
    }
}