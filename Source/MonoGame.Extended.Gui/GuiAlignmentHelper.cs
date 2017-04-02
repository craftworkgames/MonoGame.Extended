using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui
{
    public enum HorizontalAlignment { Left, Right, Centre, Stretch }
    public enum VerticalAlignment { Top, Bottom, Centre, Stretch }

    public static class GuiAlignmentHelper
    {
        public static Rectangle GetDestinationRectangle(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            var x = GetHorizontalPosition(horizontalAlignment, sourceRectangle, targetRectangle);
            var y = GetVerticalPosition(verticalAlignment, sourceRectangle, targetRectangle);
            var width = horizontalAlignment == HorizontalAlignment.Stretch ? targetRectangle.Width : sourceRectangle.Width;
            var height = verticalAlignment == VerticalAlignment.Stretch ? targetRectangle.Height : sourceRectangle.Height;

            return new Rectangle(x, y, width, height);
        }

        public static int GetHorizontalPosition(HorizontalAlignment horizontalAlignment, Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Left:
                    return targetRectangle.X;
                case HorizontalAlignment.Right:
                    return targetRectangle.Right - sourceRectangle.Width;
                case HorizontalAlignment.Centre:
                    return targetRectangle.X + targetRectangle.Width / 2 - sourceRectangle.Width / 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, $"{horizontalAlignment} is not supported");
            }
        }

        public static int GetVerticalPosition(VerticalAlignment verticalAlignment, Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                    return targetRectangle.Y;
                case VerticalAlignment.Bottom:
                    return targetRectangle.Bottom - sourceRectangle.Height;
                case VerticalAlignment.Centre:
                    return targetRectangle.Y + targetRectangle.Height / 2 - sourceRectangle.Height / 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, $"{verticalAlignment} is not supported");
            }
        }
    }
}