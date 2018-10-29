using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class Canvas : LayoutControl
    {
        public Canvas()
        {
            BackgroundColor = Color.Transparent;
        }
        
        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            foreach (var control in Items)
            {
                var actualSize = control.CalculateActualSize(context);
                PlaceControl(context, control, rectangle.X + control.Position.X, rectangle.Y+ control.Position.Y, actualSize.Width, actualSize.Height);
            }
        }

        public override Size GetContentSize(IGuiContext context)
        {
            return new Size();
        }
    }
}