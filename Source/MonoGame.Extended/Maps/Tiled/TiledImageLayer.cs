using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

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

        public Vector2 Position { get; set; }

        public override void Draw(SpriteBatch spriteBatch, Rectangle visibleRectangle)
        {
            if (!Visible)
                return;

            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}