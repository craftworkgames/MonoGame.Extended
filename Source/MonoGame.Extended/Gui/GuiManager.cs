using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Gui
{
    public class GuiManager : IDraw, IUpdate
    {
        public GuiManager(ViewportAdapter viewportAdapter, GraphicsDevice graphicsDevice)
        {
            _layout = new GuiLayout(viewportAdapter, graphicsDevice);
        }

        private readonly GuiLayout _layout;
        public GuiLayout Layout { get { return _layout; } }

        public void Draw(GameTime gameTime)
        {
            Layout.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            Layout.Update(gameTime);
        }
    }
}