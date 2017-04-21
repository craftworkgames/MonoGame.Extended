using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiCanvas : GuiLayoutControl
    {
        public GuiCanvas()
            : base(null)
        {
        }

        public GuiCanvas(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        public override void Layout(RectangleF rectangle)
        {
            foreach (var control in Controls)
            {
                control.Origin = Vector2.Zero;
                var desiredSize = control.GetMinimumSize(rectangle.Size);
                PlaceControl(control, control.Position.X, control.Position.Y, desiredSize.Width, desiredSize.Height);
            }
        }
    }
}