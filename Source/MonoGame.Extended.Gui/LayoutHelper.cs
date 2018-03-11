using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public enum HorizontalAlignment { Left, Right, Centre, Stretch }
    public enum VerticalAlignment { Top, Bottom, Centre, Stretch }

    public static class LayoutHelper
    {
        //public static Size2 GetSizeWithMargins(Control control, IGuiContext context, Size2 availableSize)
        //{
        //    return control.GetDesiredSize(context, availableSize) + control.Margin.Size;
        //}

        public static void PlaceControl(IGuiContext context, Control control, float x, float y, float width, float height)
        {
            var rectangle = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var desiredSize = control.GetActualSize(context);
            var alignedRectangle = AlignRectangle(control.HorizontalAlignment, control.VerticalAlignment, desiredSize, rectangle);

            control.Position = new Vector2(control.Margin.Left + alignedRectangle.X, control.Margin.Top + alignedRectangle.Y);
            control.Size = alignedRectangle.Size - control.Margin.Size;
            control.InvalidateMeasure();
        }

        //public static void PlaceWindow(IGuiContext context, Window window, float x, float y, float width, float height)
        //{
        //    var rectangle = new Rectangle((int)x, (int)y, (int)width, (int)height);
        //    var availableSize = new Size2(width, height);
        //    var desiredSize = window.GetDesiredSize(context, availableSize);
        //    var alignedRectangle = AlignRectangle(HorizontalAlignment.Centre, VerticalAlignment.Centre, desiredSize, rectangle);

        //    window.Position = new Vector2(alignedRectangle.X, alignedRectangle.Y);
        //    window.Size = alignedRectangle.Size;
        //    window.Layout(context, window.BoundingRectangle);
        //}

        public static Rectangle AlignRectangle(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Size2 size, Rectangle targetRectangle)
        {
            var x = GetHorizontalPosition(horizontalAlignment, size, targetRectangle);
            var y = GetVerticalPosition(verticalAlignment, size, targetRectangle);
            var width = horizontalAlignment == HorizontalAlignment.Stretch ? targetRectangle.Width : size.Width;
            var height = verticalAlignment == VerticalAlignment.Stretch ? targetRectangle.Height : size.Height;

            return new Rectangle(x, y, (int)width, (int)height);
        }

        public static int GetHorizontalPosition(HorizontalAlignment horizontalAlignment, Size2 size, Rectangle targetRectangle)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Left:
                    return targetRectangle.X;
                case HorizontalAlignment.Right:
                    return (int)(targetRectangle.Right - size.Width);
                case HorizontalAlignment.Centre:
                    return (int)(targetRectangle.X + targetRectangle.Width / 2 - size.Width / 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, $"{horizontalAlignment} is not supported");
            }
        }

        public static int GetVerticalPosition(VerticalAlignment verticalAlignment, Size2 size, Rectangle targetRectangle)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                    return targetRectangle.Y;
                case VerticalAlignment.Bottom:
                    return (int)(targetRectangle.Bottom - size.Height);
                case VerticalAlignment.Centre:
                    return (int)(targetRectangle.Y + targetRectangle.Height / 2 - size.Height / 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, $"{verticalAlignment} is not supported");
            }
        }
    }
}