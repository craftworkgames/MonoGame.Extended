using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledImageLayer : TiledLayer, IMovable
    {
        private readonly Texture2D _texture;
        private readonly SpriteBatch _spriteBatch;

        public TiledImageLayer(GraphicsDevice graphicsDevice, string name, Texture2D texture, Vector2 position)
            : base(name)
        {
            Position = position;

            _texture = texture;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }

        public Vector2 Position { get; set; }

        public override void Draw(RectangleF visibleRectangle)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_texture, Position, Color.White);
            _spriteBatch.End();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            _spriteBatch.Draw(_texture, Position + position, Color.White);
        }
    }
}