using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Maps.Tiled
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TiledMap tiledMap, Rectangle? visibleRectangle = null)
        {
            tiledMap.Draw(spriteBatch, visibleRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledMap tiledMap, Camera2D camera)
        {
            tiledMap.Draw(spriteBatch, camera);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledLayer layer, Rectangle? visibleRectangle = null)
        {
            layer.Draw(spriteBatch, visibleRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TiledLayer layer, Camera2D camera)
        {
            layer.Draw(spriteBatch, camera.GetBoundingRectangle().ToRectangle());
        }
    }
}
