using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Gui.Wip
{
    public class GuiSprite : IGuiDrawable
    {
        private NinePatch _ninePatch;

        private TextureRegion2D _textureRegion;
        public TextureRegion2D TextureRegion
        {
            get { return _textureRegion; }
            set
            {
                _textureRegion = value;
                UpdateNinePatch();
            }
        }

        private Color _color = Color.White;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateNinePatch();
            }
        }

        public GuiHorizontalAlignment HorizontalAlignment { get; set; } = GuiHorizontalAlignment.Stretch;
        public GuiVerticalAlignment VerticalAlignment { get; set; } = GuiVerticalAlignment.Stretch;
        public GuiThickness Padding { get; set; } = new GuiThickness(16);

        private void UpdateNinePatch()
        {
            _ninePatch = new NinePatch(TextureRegion, Padding.Left, Padding.Top, Padding.Right, Padding.Bottom)
            {
                Color = _color * (Color.A / 255f)
            };
        }
        
        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            var sourceRectangle = TextureRegion.Bounds;
            var targetRectangle = rectangle.ToRectangle();
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(HorizontalAlignment, VerticalAlignment, sourceRectangle, targetRectangle);

            _ninePatch.Draw(spriteBatch, destinationRectangle);
        }
    }
}