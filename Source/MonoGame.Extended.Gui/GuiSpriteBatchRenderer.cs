using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public interface IGuiRenderer
    {
        BitmapFont DefaultFont { get; }
        void Begin();
        void DrawRegion(TextureRegion2D textureRegion, Rectangle rectangle, Color color);
        void DrawRegion(TextureRegion2D textureRegion, Vector2 position, Color color);
        void DrawText(BitmapFont font, string text, Vector2 position, Color color);
        void End();
    }

    public class GuiSpriteBatchRenderer : IGuiRenderer
    {
        private readonly SpriteBatch _spriteBatch;

        public GuiSpriteBatchRenderer(GraphicsDevice graphicsDevice, BitmapFont defaultFont)
        {
            DefaultFont = defaultFont;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public BitmapFont DefaultFont { get; }
        public SpriteSortMode SortMode { get; set; }
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.Default;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullNone;
        public Effect Effect { get; set; }
        public Matrix? TransformMatrix { get; set; }

        public void Begin()
        {
            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, TransformMatrix);
        }

        public void End()
        {
            _spriteBatch.End();
        }

        public void DrawRegion(TextureRegion2D textureRegion, Rectangle rectangle, Color color)
        {
            if (textureRegion != null)
                _spriteBatch.Draw(textureRegion, rectangle, color);
            else
                _spriteBatch.FillRectangle(rectangle, color);
        }

        public void DrawRegion(TextureRegion2D textureRegion, Vector2 position, Color color)
        {
            _spriteBatch.Draw(textureRegion, position, color);
        }

        public void DrawText(BitmapFont font, string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(font, text, position, color);
        }
    }
}
