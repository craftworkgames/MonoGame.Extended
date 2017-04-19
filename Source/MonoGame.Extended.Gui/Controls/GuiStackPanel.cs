using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiStackPanel : GuiLayoutControl
    {
        public GuiStackPanel()
            : base(null)
        {
        }

        public GuiStackPanel(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        public GuiOrientation Orientation { get; set; } = GuiOrientation.Vertical;

        public override void Layout(RectangleF rectangle)
        {
            var y = rectangle.X;
            var x = rectangle.Y;
            var availableSize = rectangle.Size;

            foreach (var control in Controls)
            {
                control.Origin = Vector2.Zero;
                control.Measure(availableSize);
                var desiredSize = control.DesiredSize;

                switch (Orientation)
                {
                    case GuiOrientation.Vertical:
                        PlaceControl(control, x, y, Width, desiredSize.Height);
                        y += desiredSize.Height;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    case GuiOrientation.Horizontal:
                        PlaceControl(control, x, y, desiredSize.Width, Height);
                        x += desiredSize.Width;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }
        }
    }
}
