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
                var actualSize = control.GetActualSize(context);
                PlaceControl(context, control, control.Position.X, control.Position.Y, actualSize.Width, actualSize.Height);
            }
        }

        public override Size2 GetContentSize(IGuiContext context)
        {
            return new Size2();
        }
    }
}