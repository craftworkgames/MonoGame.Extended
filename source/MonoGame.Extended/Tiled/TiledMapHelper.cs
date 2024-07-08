using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public static class TiledMapHelper
    {
        // 4 vertices per tile
        public const int VerticesPerTile = 4;
        // 2 triangles per tile (mesh), with each triangle indexing 3 out of 4 vertices, so 6 vertices
        public const int IndicesPerTile = 6;
        // by using ushort type for indices we are limited to indexing vertices from 0 to 65535
        // this limits us on how many vertices can fit inside a single vertex buffer (65536 vertices)
        public const int MaximumVerticesPerModel = ushort.MaxValue + 1;
        // and thus, we know how many tiles we can fit inside a vertex or index buffer (16384 tiles)
        public const int MaximumTilesPerGeometryContent = MaximumVerticesPerModel / VerticesPerTile;
        // and thus, we also know the maximum number of indices we can fit inside a single index buffer (98304 indices)
        public const int MaximumIndicesPerModel = MaximumTilesPerGeometryContent * IndicesPerTile;
        // these optimal maximum numbers of course are not considering texture bindings which would practically lower the actual number of tiles per vertex / index buffer
        // thus, the reason why it is a good to have ONE giant tileset (at least per layer)

        internal static Rectangle GetTileSourceRectangle(int localTileIdentifier, int tileWidth, int tileHeight, int columns, int margin, int spacing)
        {
            var x = margin + localTileIdentifier % columns * (tileWidth + spacing);
            var y = margin + localTileIdentifier / columns * (tileHeight + spacing);
            return new Rectangle(x, y, tileWidth, tileHeight);
        }

        internal static Vector2 GetOrthogonalPosition(int tileX, int tileY, int tileWidth, int tileHeight)
        {
            var x = tileX * tileWidth;
            var y = tileY * tileHeight;
            return new Vector2(x, y);
        }

        internal static Vector2 GetIsometricPosition(int tileX, int tileY, int tileWidth, int tileHeight)
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
