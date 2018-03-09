namespace MonoGame.Extended.Gui.Controls
{
    public abstract class ContentControl : Control
    {
        public object Content { get; set; }
        public bool HasContent => Content == null;
       
        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var text = Content?.ToString();
            var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);

            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);
        }

        public override Size2 GetContentSize(IGuiContext context)
        {
            var text = Content?.ToString();
            var font = Font ?? context.DefaultFont;
            return font.MeasureString(text ?? string.Empty);
        }
    }

}