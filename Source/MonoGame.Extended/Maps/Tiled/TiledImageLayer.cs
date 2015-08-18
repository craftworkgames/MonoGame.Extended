using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledImageLayer : TiledLayer
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

        public Vector2 Position { get; set; }

        public override void Draw(Camera2D camera)
        {
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp, transformMatrix: camera.GetViewMatrix());
            _spriteBatch.Draw(_texture, Position, Color.White);
            _spriteBatch.End();
        }
    }
}