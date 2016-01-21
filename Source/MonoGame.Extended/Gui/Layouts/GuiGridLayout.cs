using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Drawables;

namespace MonoGame.Extended.Gui.Layouts
{
    public class GuiGridLayout : GuiLayoutControl
    {
        public GuiGridLayout()
        {
        }

        public override void LayoutChildren(Rectangle rectangle)
        {
            foreach (var child in Children)
            {
                var x = GetHorizontalAlignment(child, rectangle);
                var y = GetVerticalAlignment(child, rectangle);
                child.Location = new Point(x, y);
            }
        }
    }
}
