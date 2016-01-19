using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Styles.Drawables
{
    public class GuiTextureRegionDrawable : IGuiDrawable
    {
        private readonly TextureRegion2D _region;

        public GuiTextureRegionDrawable(TextureRegion2D region)
        {
            _region = region;
            Color = Color.White;
        }

        public Color Color { get; set; }

        public void Draw(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.Draw(_region, bounds, Color);
        }
    }
}