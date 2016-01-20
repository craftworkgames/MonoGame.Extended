using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Drawables;

namespace MonoGame.Extended.Gui.Layouts
{
    public class GuiGridLayout : GuiLayoutControl
    {
        public GuiGridLayout()
        {
        }

        protected override IGuiDrawable GetCurrentDrawable()
        {
            throw new NotImplementedException();
        }

        protected override void LayoutChildren(Rectangle bounds)
        {
            foreach (var child in Children)
            {
                child.Position = new Vector2(bounds.X, bounds.Y);
            }
        }
    }
}
