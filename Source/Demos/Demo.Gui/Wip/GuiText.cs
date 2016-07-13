using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;

namespace Demo.Gui.Wip
{
    public class GuiText : IGuiDrawable
    {
        public BitmapFont Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.White;
        public GuiThickness Margin { get; set; }
        public GuiHorizontalAlignment HorizontalAlignment { get; set; } = GuiHorizontalAlignment.Centre;
        public GuiVerticalAlignment VerticalAlignment { get; set; } = GuiVerticalAlignment.Centre;

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            var size = Font.MeasureString(Text);
            var sourceRectangle = new Rectangle(0, 0, size.Width, size.Height);
            var targetRectangle = rectangle.ToRectangle();
            targetRectangle = new Rectangle(
                targetRectangle.X + Margin.Left,
                targetRectangle.Y + Margin.Top,
                targetRectangle.Width - Margin.Right - Margin.Left,
                targetRectangle.Height - Margin.Bottom - Margin.Top);
            var destinationRectangle = GuiAlignmentHelper.GetDestinationRectangle(HorizontalAlignment, VerticalAlignment, sourceRectangle, targetRectangle);

            spriteBatch.DrawString(Font, Text, destinationRectangle.Location.ToVector2(), Color * (Color.A / 255f));
        }
    }
}