using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class ContentControl : Control
    {
        private bool _contentChanged = true;

        private object _content;
        public object Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    _contentChanged = true;
                }
            }
        }

        public override IEnumerable<Control> Children
        {
            get
            {
                if (Content is Control control)
                    yield return control;
            }
        }

        public bool HasContent => Content == null;

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            _contentChanged = true;
        }

        public override void Update(IGuiContext context, float deltaSeconds)
        {
            if (_content is Control control && _contentChanged)
            {
                if (_contentChanged)
                {
                    control.Parent = this;
                    control.ActualSize = ContentRectangle.Size;
                    control.Position = new Point(Padding.Left, Padding.Top);
                    if (Parent != null)
                        control.Position += this.Position;
                    control.InvalidateMeasure();
                    _contentChanged = false;
                }
            }
            else
            {
                if (_contentChanged && this.ActualSize.IsEmpty)
                {
                    this.ActualSize = CalculateActualSize(context);
                    _contentChanged = false;
                }
            }
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var control = Content as Control;

            if (control != null)
                control.Draw(context, renderer, deltaSeconds);
            else
                DrawText(context, renderer, deltaSeconds);
        }

        public virtual void DrawText(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            var text = Content?.ToString();
            var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);
            var rect = new Rectangle(ClippingRectangle.Location, ClippingRectangle.Size);
            rect.Inflate(Margin.Width, Margin.Height);
            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.Scale, rect);
        }

        public override Size GetContentSize(IGuiContext context)
        {
            if (Content is Control control)
                return control.CalculateActualSize(context);

            var text = Content?.ToString();
            var font = Font ?? context.DefaultFont;
            return (Size)font.MeasureString(text ?? string.Empty, TextScale);

        }
    }

}