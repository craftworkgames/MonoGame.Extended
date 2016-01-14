using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Gui
{
    public class GuiLayout : IDraw, IUpdate
    {
        public GuiLayout(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);

            Controls = new List<GuiControl>();
        }

        private readonly SpriteBatch _spriteBatch;

        public List<GuiControl> Controls { get; private set; }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            foreach (var control in Controls.OfType<GuiDrawableControl>())
                _spriteBatch.Draw(control.Sprite);

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var control in Controls)
                control.Update(gameTime);
        }
    }
}