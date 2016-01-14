using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Gui
{
    public class GuiManager : IDraw, IUpdate
    {
        public GuiManager(GraphicsDevice graphicsDevice)
        {
            _layout = new GuiLayout(graphicsDevice);
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