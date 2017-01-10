using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public class GuiSpriteBatchRenderer
    {
        private readonly BitmapFont _defaultFont;
        private readonly SpriteBatch _spriteBatch;

        public GuiSpriteBatchRenderer(GraphicsDevice graphicsDevice, BitmapFont defaultFont)
        {
            _defaultFont = defaultFont;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public void Draw(GuiScreen screen)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            DrawChildren(screen.Controls, Vector2.Zero);

            _spriteBatch.End();
        }

        private void DrawChildren(GuiControlCollection controls, Vector2 offset)
        {
            foreach (var control in controls)
                DrawControl(control, offset);

            foreach (var childControl in controls)
                DrawChildren(childControl.Children, offset + childControl.Position);
        }

        private void DrawControl(GuiControl control, Vector2 offset)
        {
            var location = offset + control.Position;
            var size = control.Size;

            if (control.BackgroundRegion != null)
            {
                var destinationRectangle = new Rectangle((int) location.X, (int) location.Y, (int) size.Width, (int) size.Height);
                _spriteBatch.Draw(control.BackgroundRegion, destinationRectangle, control.BackgroundColor);
            }
            else
                _spriteBatch.FillRectangle(location, size, control.BackgroundColor);

            if (_defaultFont != null && !string.IsNullOrWhiteSpace(control.Text))
            {
                var textSize = _defaultFont.MeasureString(control.Text);
                var textPosition = control.BoundingRectangle.Center - new Vector2(textSize.Width * 0.5f, textSize.Height * 0.5f);
                _spriteBatch.DrawString(_defaultFont, control.Text, textPosition, control.TextColor);
            }
        }
    }
}