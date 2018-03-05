namespace MonoGame.Extended.Gui.Controls
{
    public class Canvas : LayoutControl
    {
        public Canvas()
        {
        }
        
        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            foreach (var control in Items)
            {
                var desiredSize = control.GetDesiredSize(context, rectangle.Size);
                PlaceControl(context, control, control.Position.X, control.Position.Y, desiredSize.Width, desiredSize.Height);
            }
        }
    }
}