using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Gui
{
    public class GuiDialog : IMovable, ISizable
    {
        private GuiLayout _layout;

        public GuiDialog()
        {
        }

        public Vector2 Position { get; set; }

        public SizeF Size { get; set; }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_layout == null)
                return;

            foreach (var control in _layout.Controls)
                control.Draw(spriteBatch);
        }

        public bool Contains(Point point)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 point)
        {
            throw new NotImplementedException();
        }
    }
}