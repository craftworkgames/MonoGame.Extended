using System.Linq;
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
        private readonly RasterizerState _rasterizerState;

        public GuiSpriteBatchRenderer(GraphicsDevice graphicsDevice, BitmapFont defaultFont)
        {
            _defaultFont = defaultFont;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _rasterizerState = new RasterizerState
            {
                ScissorTestEnable = true
            };
        }

        public void Draw(GuiScreen screen)
        {
            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, TransformMatrix);

            DrawChildren(screen.Controls);

            _spriteBatch.End();
        }

        public SpriteSortMode SortMode { get; set; }
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;
        public Effect Effect { get; set; }
        public Matrix? TransformMatrix { get; set; }

        private void DrawChildren(GuiControlCollection controls)
        {
            foreach (var control in controls.Where(c => c.IsVisible))
                DrawControl(control);

            foreach (var childControl in controls.Where(c => c.IsVisible))
                DrawChildren(childControl.Controls);
        }

        private void DrawControl(GuiControl control)
        {
            //if (control is GuiProgressBar)
            //{
            //    _spriteBatch.End();

            //    var bar = (GuiProgressBar) control;
            //    _spriteBatch.GraphicsDevice.ScissorRectangle = new RectangleF(control.BoundingRectangle.X,
            //            control.BoundingRectangle.Y, control.BoundingRectangle.Width * bar.Progress, control.BoundingRectangle.Height)
            //        .ToRectangle();

            //    _spriteBatch.Begin(rasterizerState: _rasterizerState);

            //    if (control.TextureRegion != null)
            //        _spriteBatch.Draw(control.TextureRegion, control.BoundingRectangle.ToRectangle(), control.Color);
            //    else
            //        _spriteBatch.FillRectangle(control.BoundingRectangle, control.Color);

            //    _spriteBatch.End();

            //    _spriteBatch.Begin(rasterizerState: RasterizerState);
            //}
            //else
            //{
                if (control.TextureRegion != null)
                    _spriteBatch.Draw(control.TextureRegion, control.BoundingRectangle.ToRectangle(), control.Color);
                else
                    _spriteBatch.FillRectangle(control.BoundingRectangle, control.Color);

            //}

            if (_defaultFont != null && !string.IsNullOrWhiteSpace(control.Text))
            {
                var textSize = _defaultFont.MeasureString(control.Text);
                var textPosition = control.BoundingRectangle.Center - new Vector2(textSize.Width * 0.5f, textSize.Height * 0.5f);
                _spriteBatch.DrawString(_defaultFont, control.Text, textPosition + control.TextOffset, control.TextColor);
            }
        }
    }
}