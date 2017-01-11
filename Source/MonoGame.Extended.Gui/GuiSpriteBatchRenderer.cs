using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public interface IGuiRenderer
    {
        void Draw(GuiScreen screen);
    }

    public class GuiSpriteBatchRenderer : IGuiRenderer
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
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            DrawChildren(screen.Controls);

            _spriteBatch.End();
        }

        private void DrawChildren(GuiControlCollection controls)
        {
            foreach (var control in controls)
                DrawControl(control);

            foreach (var childControl in controls)
                DrawChildren(childControl.Children);
        }

        private void DrawControl(GuiControl control)
        {
            if (control.TextureRegion != null)
                _spriteBatch.Draw(control.TextureRegion, control.BoundingRectangle.ToRectangle(), control.Color);
            else
                _spriteBatch.FillRectangle(control.BoundingRectangle, control.Color);

            if (_defaultFont != null && !string.IsNullOrWhiteSpace(control.Text))
            {
                var textSize = _defaultFont.MeasureString(control.Text);
                var textPosition = control.BoundingRectangle.Center - new Vector2(textSize.Width * 0.5f, textSize.Height * 0.5f);
                _spriteBatch.DrawString(_defaultFont, control.Text, textPosition, control.TextColor);
            }
        }
    }
}