using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public enum GuiOrientation { Horizontal, Vertical }

    internal static class GuiLayoutExtensions
    {
        public static void SetPositionWithMargins(this GuiControl control, float x, float y)
        {
            control.Position = new Vector2(x + control.Margin.Left, y + control.Margin.Top);
        }

        public static void SetSizeWithMargins(this GuiControl control, float width, float height)
        {
            control.Size = new Size2(width - control.Margin.Left - control.Margin.Right, height - control.Margin.Top - control.Margin.Bottom);
        }

        public static Size2 GetSizeWithMargins(this GuiControl control)
        {
            return new Size2(control.Size.Width + control.Margin.Left + control.Margin.Right, control.Size.Height + control.Margin.Top + control.Margin.Bottom);
        }

        public static Vector2 GetPositionWithMargins(this GuiControl control)
        {
            return new Vector2(control.Position.X - control.Margin.Left, control.Position.Y - control.Margin.Top);
        }
    }

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

        public override void PerformLayout()
        {
            var y = 0f;
            var x = 0f;

            foreach (var control in Controls)
            {
                control.Origin = Vector2.Zero;
                control.SetPositionWithMargins(x, y);

                switch (Orientation)
                {
                    case GuiOrientation.Vertical:
                        control.SetSizeWithMargins(Width, control.Height);
                        y += control.GetSizeWithMargins().Height;
                        break;
                    case GuiOrientation.Horizontal:
                        control.SetSizeWithMargins(control.Width, Height);
                        x += control.GetSizeWithMargins().Width;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }
        }

    }
}
