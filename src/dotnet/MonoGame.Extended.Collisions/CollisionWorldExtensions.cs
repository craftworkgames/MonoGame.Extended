//using System.Linq;
//using MonoGame.Extended.Tiled;

//namespace MonoGame.Extended.Collisions
//{
//    public static class CollisionWorldExtensions
//    {
//        public static CollisionGrid CreateGrid(this CollisionWorld world, TiledTileLayer tileLayer)
//        {
//            var data = tileLayer.Tiles
//                .Select(t => (int)t.GlobalIdentifier)
//                .ToArray();

//            return world.CreateGrid(data, tileLayer.Width, tileLayer.Height, tileLayer.TileWidth, tileLayer.TileHeight);
//        }
//    }
//}