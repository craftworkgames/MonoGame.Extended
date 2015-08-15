using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledImageLayer : TiledLayer
    {
        private readonly Texture2D _texture;
        private readonly SpriteBatch _spriteBatch;

        public TiledImageLayer(GraphicsDevice graphicsDevice, string name, Texture2D texture)
            : base(name)
        {
            _texture = texture;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Draw(Camera2D camera)
        {
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp, transformMatrix: camera.GetViewMatrix());
            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}