using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    internal static class TiledMapHelper
    {
        internal static Rectangle GetTileSourceRectangle(int localTileIdentifier, int tileWidth, int tileHeight,
            int columns, int margin, int spacing)
        {
            var x = margin + localTileIdentifier % columns * (tileWidth + spacing);
            var y = margin + localTileIdentifier / columns * (tileHeight + spacing);
            return new Rectangle(x, y, tileWidth, tileHeight);
        }

        internal static Point2 GetOrthogonalPosition(int tileX, int tileY, int tileWidth, int tileHeight)
        {
            var x = tileX * tileWidth;
            var y = tileY * tileHeight;
            return new Vector2(x, y);
        }

        internal static Point2 GetIsometricPosition(int tileX, int tileY, int tileWidth, int tileHeight)
        {
            // You can think of an isometric Tiled map as a regular orthogonal map that is rotated -45 degrees
            // i.e.: the origin (0, 0) is the top tile of the diamond grid; 
            //                  (mapWidth, 0) is the far right tile of the diamond grid
            //                  (0, mapHeight) is the far left tile of the diamond grid
            //                  (mapWidth, mapHeight) is the bottom tile of the diamond grid 

            var halfTileWidth = tileWidth * 0.5f;
            var halfTileHeight = tileHeight * 0.5f;
            // -1 because we want the top the tile-diamond (top-center) to be the origin in tile space
            var x = (tileX - tileY - 1) * halfTileWidth;
            var y = (tileX + tileY) * halfTileHeight;
            return new Vector2(x, y);
        }
    }
}
