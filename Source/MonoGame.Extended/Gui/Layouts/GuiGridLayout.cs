using Microsoft.Xna.Framework;

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
