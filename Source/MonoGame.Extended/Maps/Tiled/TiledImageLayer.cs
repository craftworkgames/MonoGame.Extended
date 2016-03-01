using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledImageLayer : TiledLayer, IMovable
    {
        private readonly Texture2D _texture;

        public TiledImageLayer(string name, Texture2D texture, Vector2 position)
            : base(name)
        {
            Position = position;
            _texture = texture;
        }

        public override void Dispose()
        {
        }

        public Vector2 Position { get; set; }

        public override void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(_texture, Position, Color.White * Opacity);
        }
    }
}