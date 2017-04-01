using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public interface IGuiRenderer
    {
        void Begin();
        void DrawRegion(TextureRegion2D textureRegion, Rectangle rectangle, Color color, Rectangle? clippingRectangle = null);
        void DrawRegion(TextureRegion2D textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null);
        void DrawText(BitmapFont font, string text, Vector2 position, Color color, Rectangle? clippingRectangle = null);
        void DrawRectangle(Rectangle rectangle, Color color, float thickness = 1f, Rectangle? clippingRectangle = null);
        void FillRectangle(Rectangle rectangle, Color color, Rectangle? clippingRectangle = null);
        void End();
    }

    public class GuiSpriteBatchRenderer : IGuiRenderer
    {
        private readonly Func<Matrix> _getTransformMatrix;
        private readonly SpriteBatch _spriteBatch;

        public GuiSpriteBatchRenderer(GraphicsDevice graphicsDevice, Func<Matrix> getTransformMatrix)
        {
            _getTransformMatrix = getTransformMatrix;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public SpriteSortMode SortMode { get; set; }
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;
        public Effect Effect { get; set; }

        public void Begin()
        {
            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, _getTransformMatrix());
        }

        public void End()
        {
            _spriteBatch.End();
        }

        public void DrawRegion(TextureRegion2D textureRegion, Rectangle rectangle, Color color, Rectangle? clippingRectangle = null)
        {
            if(textureRegion != null)
                _spriteBatch.Draw(textureRegion, rectangle, color, clippingRectangle);
        }

        public void DrawRegion(TextureRegion2D textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            if (textureRegion != null)
                _spriteBatch.Draw(textureRegion, position, color, clippingRectangle);
        }

        public void DrawText(BitmapFont font, string text, Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            _spriteBatch.DrawString(font, text, position, color, clippingRectangle);
        }

        public void DrawRectangle(Rectangle rectangle, Color color, float thickness = 1f, Rectangle? clippingRectangle = null)
        {
            if (clippingRectangle.HasValue)
                rectangle = rectangle.Clip(clippingRectangle.Value);

            _spriteBatch.DrawRectangle(rectangle, color, thickness);
        }

        public void FillRectangle(Rectangle rectangle, Color color, Rectangle? clippingRectangle = null)
        {
            if (clippingRectangle.HasValue)
                rectangle = rectangle.Clip(clippingRectangle.Value);

            _spriteBatch.FillRectangle(rectangle, color);
        }
    }
}
