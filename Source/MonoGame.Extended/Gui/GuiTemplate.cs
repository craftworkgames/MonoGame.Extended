using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Drawables;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui
{
    public class GuiTemplate
    {
        public GuiTemplate()
        {
            Drawables = new List<IGuiDrawable>();
        }

        public List<IGuiDrawable> Drawables { get; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            foreach (var drawable in Drawables)
                drawable.Draw(spriteBatch, rectangle);
        }
    }
}