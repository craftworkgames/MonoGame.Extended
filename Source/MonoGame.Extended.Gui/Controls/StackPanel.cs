using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class StackPanel : LayoutControl
    {
        public StackPanel()
        {
            Items.ItemAdded += OnItemAdded;
        }

        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public int Spacing { get; set; }

        private void OnItemAdded(Control control)
        {
            base.IsLayoutRequired = true;
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var width = 0;
            var height = 0;

            foreach (var control in Items.GetList())
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
            var x = rectangle.X;
            var y = rectangle.Y;

            bool firstColumn = true;
            var rowCount = 0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                var control = this.Items[i];
                var actualSize = control.CalculateActualSize(context);

                switch (Orientation)
                {
                    case Orientation.Vertical:
                        PlaceControl(context, control, x, y, rectangle.Width, actualSize.Height);
                        y += actualSize.Height + Spacing;
                        //rectangle.Height -= actualSize.Height;
                        break;
                    case Orientation.Horizontal:
                        if (x + actualSize.Width > rectangle.Right)
                        {
                            firstColumn = true;
                            x = this.Position.X;
                            rowCount += 1;
                        }

                        y = this.Position.Y+ rowCount * actualSize.Height;

                        if (y + actualSize.Height > rectangle.Bottom)
                        {
                            this.Height += y + actualSize.Height; //expand the height
                            this.ActualSize = new Size(this.ActualSize.Width, this.Height);
                        }

                        if (firstColumn)
                            x += GetX(context, rectangle.Size);

                        PlaceControl(context, control, x, y, actualSize.Width, rectangle.Height);

                        x += actualSize.Width;
                        firstColumn = false;

                        //PlaceControl(context, control, rectangle.X, rectangle.Y, actualSize.Width, rectangle.Height);
                        //rectangle.X += actualSize.Width + Spacing;
                        //rectangle.Width -= actualSize.Width;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }

            }
            IsLayoutRequired = false;
            
        }

        private int GetX(IGuiContext context, Size availableSize)
        {
            if (HorizontalAlignment == HorizontalAlignment.Left)
                return 0;

            int x = 0;
            var desiredSize = new Size2(0, 0);

            // Get the width of the controls that can fit in the row
            foreach (var control in Items)
            {
                Size size = size = control.CalculateActualSize(context);

                desiredSize.Height = Math.Max(size.Height, desiredSize.Height);
                desiredSize.Width += size.Width;
                if (desiredSize.Width > availableSize.Width)
                {
                    desiredSize.Width -= size.Width;
                    break;
                }
            }

            if (HorizontalAlignment == HorizontalAlignment.Centre)
                x = (int)(availableSize.Width / 2 - desiredSize.Width / 2);
            else if (HorizontalAlignment == HorizontalAlignment.Right)
                x += (int)(availableSize.Width - desiredSize.Width);
            else if (HorizontalAlignment == HorizontalAlignment.Stretch)
                x = (int)(availableSize.Width / 2 - desiredSize.Width / 2);

            x += 0;
            return x;
        }

    }
}
