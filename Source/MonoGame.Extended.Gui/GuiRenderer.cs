using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

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
        private readonly BitmapFont _defaultFont;

        public GuiSpriteBatchRenderer(GuiScreen targetScreen, BitmapFont defaultFont = null)
            : base(targetScreen)
        {
            _defaultFont = defaultFont;
        }

        protected override void DrawControl(SpriteBatch spriteBatch, GuiControl control)
        {
            var destinationRectangle = control.DestinationRectangle;

            DrawBackground(spriteBatch, control, destinationRectangle);
            DrawText(spriteBatch, control, destinationRectangle);
        }

        private void DrawText(SpriteBatch spriteBatch, GuiControl control, Rectangle rectangle)
        {
            var font = control.Font ?? _defaultFont;

            if (font == null)
                return;

            var size = font.MeasureString(control.Text);
            var sourceRectangle = new Rectangle(0, 0, size.Width, size.Height);
            var targetRectangle = rectangle;
            targetRectangle = new Rectangle(
                targetRectangle.X + control.Margin.Left,
                targetRectangle.Y + control.Margin.Top,
                targetRectangle.Width - control.Margin.Right - control.Margin.Left,
                targetRectangle.Height - control.Margin.Bottom - control.Margin.Top);
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                control.HorizontalTextAlignment, control.VerticalTextAlignment, sourceRectangle, targetRectangle);

            spriteBatch.DrawString(font, control.Text, destinationRectangle.Location.ToVector2(), control.TextColor * (control.TextColor.A / 255f));
        }

        private static void DrawBackground(SpriteBatch spriteBatch, GuiControl control, Rectangle rectangle)
        {
            if (control.BackgroundRegion == null)
            {
                spriteBatch.FillRectangle(rectangle, control.BackgroundColor);
            }
            else
            {
                var targetRectangle = rectangle;
                var sourceRectangle = control.BackgroundRegion.Bounds;
                var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(
                    control.HorizontalAlignment, control.VerticalAlignment, sourceRectangle, targetRectangle);
                var color = control.BackgroundColor * (control.BackgroundColor.A / 255f);

                spriteBatch.Draw(control.BackgroundRegion, destinationRectangle, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawControls(spriteBatch, TargetScreen.Controls);
        }

        private void DrawControls(SpriteBatch spriteBatch, IEnumerable<GuiControl> controls)
        {
            foreach (var control in controls)
            {
                DrawControl(spriteBatch, control);

                var panel = control as GuiPanel;

                if (panel != null)
                    DrawControls(spriteBatch, panel.Controls);
            }
        }
    }
}