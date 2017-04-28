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

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var width = 0f;
            var height = 0f;

            foreach (var control in Controls)
            {
                var desiredSize = GuiLayoutHelper.GetSizeWithMargins(control, context, availableSize);

                switch (Orientation)
                {
                    case GuiOrientation.Horizontal:
                        width += desiredSize.Width;
                        height = desiredSize.Height > height ? desiredSize.Height : height;
                        break;
                    case GuiOrientation.Vertical:
                        width = desiredSize.Width > width ? desiredSize.Width : width;
                        height += desiredSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }

            return new Size2(width, height);
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var x = 0f;
            var y = 0f;
            var availableSize = rectangle.Size;

            foreach (var control in Controls)
            {
                var desiredSize = GuiLayoutHelper.GetSizeWithMargins(control, context, availableSize);

                switch (Orientation)
                {
                    case GuiOrientation.Vertical:
                        PlaceControl(context, control, x, y, Width, desiredSize.Height);
                        y += desiredSize.Height;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    case GuiOrientation.Horizontal:
                        PlaceControl(context, control, x, y, desiredSize.Width, Height);
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
