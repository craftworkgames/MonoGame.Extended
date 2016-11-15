using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui
{
    public enum GuiHorizontalAlignment
    {
        Left,
        Right,
        Centre,
        Stretch
    }

    public enum GuiVerticalAlignment
    {
        Top,
        Bottom,
        Centre,
        Stretch
    }

    public static class GuiAlignmentHelper
    {
        public static Rectangle GetDestinationRectangle(GuiHorizontalAlignment horizontalAlignment,
            GuiVerticalAlignment verticalAlignment,
            Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            var x = GetHorizontalPosition(horizontalAlignment, sourceRectangle, targetRectangle);
            var y = GetVerticalPosition(verticalAlignment, sourceRectangle, targetRectangle);
            var width = horizontalAlignment == GuiHorizontalAlignment.Stretch
                ? targetRectangle.Width
                : sourceRectangle.Width;
            var height = verticalAlignment == GuiVerticalAlignment.Stretch
                ? targetRectangle.Height
                : sourceRectangle.Height;
            return new Rectangle(x, y, width, height);
        }

        public static int GetHorizontalPosition(GuiHorizontalAlignment horizontalAlignment, Rectangle sourceRectangle,
            Rectangle targetRectangle)
        {
            switch (horizontalAlignment)
            {
                case GuiHorizontalAlignment.Stretch:
                case GuiHorizontalAlignment.Left:
                    return targetRectangle.X;
                case GuiHorizontalAlignment.Right:
                    return targetRectangle.Right - sourceRectangle.Width;
                case GuiHorizontalAlignment.Centre:
                    return targetRectangle.X + targetRectangle.Width/2 - sourceRectangle.Width/2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment,
                        $"{horizontalAlignment} is not supported");
            }
        }

        public static int GetVerticalPosition(GuiVerticalAlignment verticalAlignment, Rectangle sourceRectangle,
            Rectangle targetRectangle)
        {
            switch (verticalAlignment)
            {
                case GuiVerticalAlignment.Stretch:
                case GuiVerticalAlignment.Top:
                    return targetRectangle.Y;
                case GuiVerticalAlignment.Bottom:
                    return targetRectangle.Bottom - sourceRectangle.Height;
                case GuiVerticalAlignment.Centre:
                    return targetRectangle.Y + targetRectangle.Height/2 - sourceRectangle.Height/2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment,
                        $"{verticalAlignment} is not supported");
            }
        }
    }
}