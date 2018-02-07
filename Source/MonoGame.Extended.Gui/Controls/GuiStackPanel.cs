using System;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiStackPanel : GuiLayoutControl
    {
        public GuiStackPanel()
            : this(null)
        {
        }

        public GuiStackPanel(GuiSkin skin) 
            : base(skin)
        {
            HorizontalAlignment = HorizontalAlignment.Centre;
            VerticalAlignment = VerticalAlignment.Centre;
        }

        public GuiOrientation Orientation { get; set; } = GuiOrientation.Vertical;
        public Thickness Padding { get; set; }
        public int Spacing { get; set; } = 0;

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


            width += Padding.Left + Padding.Right + (Orientation == GuiOrientation.Horizontal ? (Controls.Count - 1) * Spacing : 0);
            height += Padding.Top + Padding.Bottom + (Orientation == GuiOrientation.Vertical ? (Controls.Count - 1) * Spacing : 0);

            return new Size2(width, height);
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var x = (float)Padding.Left;
            var y = (float)Padding.Top;
            var availableSize = rectangle.Size;

            foreach (var control in Controls)
            {
                var desiredSize = GuiLayoutHelper.GetSizeWithMargins(control, context, availableSize);

                switch (Orientation)
                {
                    case GuiOrientation.Vertical:
                        control.VerticalAlignment = VerticalAlignment.Top;

                        PlaceControl(context, control, 0f, y, Width, desiredSize.Height);
                        y += desiredSize.Height + Spacing;
                        availableSize.Height -= desiredSize.Height;
                        break;
                    case GuiOrientation.Horizontal:
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
