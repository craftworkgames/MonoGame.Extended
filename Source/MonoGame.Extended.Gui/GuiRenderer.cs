using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiRenderer<T>
    {
        protected GuiRenderer(GuiScreen targetScreen)
        {
            TargetScreen = targetScreen;
        }

        public GuiScreen TargetScreen { get; set; }

        protected abstract void DrawControl(T drawer, GuiControl control);
    }

    public class GuiSpriteBatchRenderer : GuiRenderer<SpriteBatch>
    {
        public GuiSpriteBatchRenderer(GuiScreen targetScreen)
            : base(targetScreen)
        {
        }

        protected override void DrawControl(SpriteBatch spriteBatch, GuiControl control)
        {
            var destinationRectangle = control.DestinationRectangle;

            DrawBackground(spriteBatch, control, destinationRectangle);
            DrawText(spriteBatch, control, destinationRectangle);
        }

        private static void DrawText(SpriteBatch spriteBatch, GuiControl control, Rectangle rectangle)
        {
            if (control.Font == null)
                return;

            var size = control.Font.MeasureString(control.Text);
            var sourceRectangle = new Rectangle(0, 0, size.Width, size.Height);
            var targetRectangle = rectangle;
            targetRectangle = new Rectangle(
                targetRectangle.X + control.Margin.Left,
                targetRectangle.Y + control.Margin.Top,
                targetRectangle.Width - control.Margin.Right - control.Margin.Left,
                targetRectangle.Height - control.Margin.Bottom - control.Margin.Top);
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                control.HorizontalTextAlignment, control.VerticalTextAlignment, sourceRectangle, targetRectangle);

            spriteBatch.DrawString(control.Font, control.Text, destinationRectangle.Location.ToVector2(), control.TextColor * (control.TextColor.A / 255f));
        }

        private static void DrawBackground(SpriteBatch spriteBatch, GuiControl control, Rectangle rectangle)
        {
            if (control.BackgroundRegion == null)
            {
                spriteBatch.FillRectangle(control.Position, control.Size, control.BackgroundColor);
            }
            else
            {
                var targetRectangle = rectangle;
                var sourceRectangle = control.BackgroundRegion.Bounds;
                var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                    control.HorizontalAlignment, control.VerticalAlignment, sourceRectangle, targetRectangle);
                var color = control.BackgroundColor * (control.BackgroundColor.A / 255f);
                var ninePatch = new NinePatch(control.BackgroundRegion, control.Padding.Left, control.Padding.Top,
                    control.Padding.Right, control.Padding.Bottom);
                ninePatch.Draw(spriteBatch, destinationRectangle, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var control in TargetScreen.Controls)
                DrawControl(spriteBatch, control);
        }
    }
}