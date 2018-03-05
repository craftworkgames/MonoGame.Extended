
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class LayoutControl : Control
    {
        protected LayoutControl()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Color = Color.Transparent;
        }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            return availableSize;
        }

        public abstract void Layout(IGuiContext context, RectangleF rectangle);

        protected static void PlaceControl(IGuiContext context, Control control, float x, float y, float width, float height)
        {
            LayoutHelper.PlaceControl(context, control, x, y, width, height);
        }
    }
}