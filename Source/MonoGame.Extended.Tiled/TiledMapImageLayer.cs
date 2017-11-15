using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapImageLayer : TiledMapLayer, IMovable
    {
        public TiledMapImageLayer(string name, Texture2D image, Vector2? position = null, Vector2? offset = null, float opacity = 1.0f, bool isVisible = true)
            : base(name, offset, opacity, isVisible)
        {
            Image = image;
            Position = position ?? Vector2.Zero;
        }

        public Texture2D Image { get; }
        public Vector2 Position { get; set; }
    }
}