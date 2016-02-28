using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TiledMap tiledMap, Camera2D camera, bool useMapBackgroundColor = false)
        {
            tiledMap.Draw(spriteBatch, camera, useMapBackgroundColor);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledTileLayer tileLayer, Rectangle? visibleRectangle = null)
        {
            visibleRectangle = visibleRectangle ?? new Rectangle (0, 0, tileLayer.Width * tileLayer.TileWidth, tileLayer.Height * tileLayer.TileHeight);

            tileLayer.Draw(spriteBatch, (Rectangle)visibleRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledTileLayer tileLayer, Rectangle visibleRectangle)
        {
            tileLayer.Draw(spriteBatch, visibleRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledMap tileMap, Rectangle? visibleRectangle = null)
        {
            visibleRectangle = visibleRectangle ?? new Rectangle (0, 0, tileMap.Width * tileMap.TileWidth, tileMap.Height * tileMap.TileHeight);

            tileMap.Draw(spriteBatch, (Rectangle)visibleRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledMap tileMap, Rectangle visibleRectangle)
        {
            tileMap.Draw(spriteBatch, visibleRectangle);
        }
    }
}
