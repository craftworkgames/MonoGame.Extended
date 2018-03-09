using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class ContentControl : Control
    {
        public object Content { get; set; }
        public bool HasContent => Content == null;
        public Thickness Padding { get; set; }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var font = Font ?? context.DefaultFont;
            var fontSize = Size2.Empty;

            if (font != null && Content != null)
            {
                //var blockText = CreateBlockText(, font, Width);
                var textSize = font.MeasureString(Content.ToString());
                fontSize.Width += textSize.Width;
                fontSize.Height += textSize.Height;
            }
            
            return fontSize + Padding.Size;
        }
        
        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            //var text = CreateBlockText(Content?.ToString(), Font ?? context.DefaultFont, Width);
            var text = Content?.ToString();
            var textInfo = GetTextInfo(context, text, BoundingRectangle, HorizontalTextAlignment, VerticalTextAlignment);

            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);
        }

        public override Size2 GetContentSize(IGuiContext context)
        {
            var text = Content?.ToString();
            var font = Font ?? context.DefaultFont;
            return font.GetStringRectangle(text ?? string.Empty, Vector2.Zero).Size;
        }

        //protected virtual string CreateBlockText(string text, BitmapFont font, float width)
        //{
        //    if (string.IsNullOrEmpty(text) || width <= 0.0f) return text;
        //    var words = text.Split(' ');
        //    var currentWidth = 0;

        //    var blockText = string.Empty;
        //    foreach (var word in words)
        //    {
        //        var spaceMeasurement = font.MeasureString($" {word}");
        //        currentWidth += (int)spaceMeasurement.Width;

        //        if (currentWidth > width)
        //        {
        //            var measurement = font.MeasureString(word);
        //            blockText += string.IsNullOrEmpty(blockText) ? word : $"\n{word}";
        //            currentWidth = (int)measurement.Width;
        //        }
        //        else
        //        {
        //            blockText += string.IsNullOrEmpty(blockText) ? word : $" {word}";
        //        }
        //    }
        //    return blockText;
        //}

    }

}