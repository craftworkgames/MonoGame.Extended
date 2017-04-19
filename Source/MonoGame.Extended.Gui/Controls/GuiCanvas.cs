using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiCanvas : GuiLayoutControl
    {
        public override void Layout(RectangleF rectangle)
        {
            foreach (var control in Controls)
            {
                control.Origin = Vector2.Zero;
                control.Measure(rectangle.Size);
                var desiredSize = control.DesiredSize;
                PlaceControl(control, control.Position.X, control.Position.Y, desiredSize.Width, desiredSize.Height);
            }
        }
    }
}