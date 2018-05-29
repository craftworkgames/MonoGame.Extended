using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class Canvas : LayoutControl
    {
        public Canvas()
        {
        }
        
        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            foreach (var control in Items)
            {
                var actualSize = control.CalculateActualSize(context);
                PlaceControl(context, control, control.Position.X, control.Position.Y, actualSize.Width, actualSize.Height);
            }
        }

        public override Size GetContentSize(IGuiContext context)
        {
            return new Size();
        }
    }
}