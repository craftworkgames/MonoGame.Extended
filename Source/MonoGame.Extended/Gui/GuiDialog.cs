using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Gui
{
    public class GuiDialog : IMovable, ISizable
    {
        GuiLayout _layout;
        
        public SizeF Size { get; set; }
        public Vector2 Position { get; set; }

        public GuiDialog()
        {

        }

        #region Delegates



        #endregion

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
            throw new System.NotImplementedException();
        }

        public bool Contains(Vector2 point)
        {
            throw new System.NotImplementedException();
        }
    }
}
