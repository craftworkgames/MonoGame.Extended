using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class StackPanel : LayoutControl
    {
        public StackPanel()
        {
        }

        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public int Spacing { get; set; }

        public override Size GetContentSize(IGuiContext context)
        {
            var width = 0;
            var height = 0;

            foreach (var control in Items)
            {
                var actualSize = control.CalculateActualSize(context);

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        width += actualSize.Width;
                        height = actualSize.Height > height ? actualSize.Height : height;
                        break;
                    case Orientation.Vertical:
                        width = actualSize.Width > width ? actualSize.Width : width;
                        height += actualSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }

            width += Orientation == Orientation.Horizontal ? (Items.Count - 1) * Spacing : 0;
            height += Orientation == Orientation.Vertical ? (Items.Count - 1) * Spacing : 0;

            return new Size(width, height);
        }

        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            foreach (var control in Items)
            {
                var actualSize = control.CalculateActualSize(context);

                switch (Orientation)
                {
                    case Orientation.Vertical:
                        PlaceControl(context, control, rectangle.X, rectangle.Y, rectangle.Width, actualSize.Height);
                        rectangle.Y += actualSize.Height + Spacing;
                        rectangle.Height -= actualSize.Height;
                        break;
                    case Orientation.Horizontal:
                        PlaceControl(context, control, rectangle.X, rectangle.Y, actualSize.Width, rectangle.Height);
                        rectangle.X += actualSize.Width + Spacing;
                        rectangle.Width -= actualSize.Width;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }
        }
    }
}
