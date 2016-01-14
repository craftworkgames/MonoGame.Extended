using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiLayout : IDraw, IUpdate
    {
        private readonly ViewportAdapter _viewportAdapter;

        public GuiLayout(ViewportAdapter viewportAdapter, GraphicsDevice graphicsDevice)
        {
            _viewportAdapter = viewportAdapter;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            Controls = new List<GuiControl>();
        }

        private readonly SpriteBatch _spriteBatch;

        public List<GuiControl> Controls { get; private set; }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());

            foreach (var control in Controls.OfType<GuiDrawableControl>())
                control.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var control in Controls)
                control.Update(gameTime);
        }
    }
}