using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.TextureAtlases;

namespace JamGame.Systems
{
    public class HudSystem : DrawSystem
    {
        private readonly MainGame _game;
        private readonly BitmapFont _font;
        private readonly Tileset _tileset;
        private readonly SpriteBatch _spriteBatch;

        public HudSystem(MainGame game, GraphicsDevice graphicsDevice, BitmapFont font, Tileset tileset)
        {
            _game = game;
            _font = font;
            _tileset = tileset;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var tx = _game.MouseState.X / _tileset.TileWidth / 2;
            var ty = _game.MouseState.Y / _tileset.TileHeight / 2;
            var region = _tileset.GetTileAt(tx, ty);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(2));
            _spriteBatch.DrawString(_font, $"{tx}, {ty}", Vector2.Zero, Color.AliceBlue);
            _spriteBatch.DrawRectangle(tx * _tileset.TileWidth, ty * _tileset.TileHeight, _tileset.TileWidth, _tileset.TileHeight, Color.White);

            if(region != null)
                _spriteBatch.Draw(region, new Vector2(350, 0), Color.White);

            _spriteBatch.End();
        }

    }
}