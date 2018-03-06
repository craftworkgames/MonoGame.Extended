using System;

namespace MonoGame.Extended.Gui.Controls
{
    public class StackPanel : LayoutControl
    {
        public StackPanel()
        {
        }

        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public int Spacing { get; set; } = 0;

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var width = 0f;
            var height = 0f;

            foreach (var control in Items)
            {
                var desiredSize = LayoutHelper.GetSizeWithMargins(control, context, availableSize);

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        width += desiredSize.Width;
                        height = desiredSize.Height > height ? desiredSize.Height : height;
                        break;
                    case Orientation.Vertical:
                        width = desiredSize.Width > width ? desiredSize.Width : width;
                        height += desiredSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }


            //width += Padding.Left + Padding.Right + (Orientation == Orientation.Horizontal ? (Items.Count - 1) * Spacing : 0);
            //height += Padding.Top + Padding.Bottom + (Orientation == Orientation.Vertical ? (Items.Count - 1) * Spacing : 0);

            return new Size2(width, height);
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var x = 0f;//(float)Padding.Left;
            var y = 0f;//(float)Padding.Top;
            var availableSize = rectangle.Size;

            foreach (var control in Items)
            {
                var desiredSize = LayoutHelper.GetSizeWithMargins(control, context, availableSize);

                switch (Orientation)
                {
                    case Orientation.Vertical:
                        control.VerticalAlignment = VerticalAlignment.Top;

                        PlaceControl(context, control, 0f, y, Width, desiredSize.Height);
                        y += desiredSize.Height + Spacing;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    case Orientation.Horizontal:
                        control.HorizontalAlignment = HorizontalAlignment.Left;

                        PlaceControl(context, control, x, 0f, desiredSize.Width, Height);
                        x += desiredSize.Width + Spacing;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }
        }
    }
}
