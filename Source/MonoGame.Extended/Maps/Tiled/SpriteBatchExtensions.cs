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

        public static void Draw(this SpriteBatch spriteBatch, TiledLayer layer, Rectangle visibleRectangle)
        {
            layer.Draw(spriteBatch, visibleRectangle);
        }
    }
}
