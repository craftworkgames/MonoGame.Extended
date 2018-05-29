﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class ContentControl : Control
    {
        private bool _contentChanged = true;

        private object _content;
        public object Content
        {
            get { return _content; }
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
                var control = Content as Control;

                if (control != null)
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
            var control = _content as Control;

            if (control != null)
            {
                if (_contentChanged)
                {
                    control.Parent = this;
                    control.ActualSize = ContentRectangle.Size;
                    control.Position = new Point(Padding.Left, Padding.Top);
                    control.InvalidateMeasure();
                    _contentChanged = false;
                }
            }
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var control = Content as Control;

            if (control != null)
            {
                control.Draw(context, renderer, deltaSeconds);
            }
            else
            {
                var text = Content?.ToString();
                var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);

                if (!string.IsNullOrWhiteSpace(textInfo.Text))
                    renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);
            }
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var control = Content as Control;

            if (control != null)
                return control.CalculateActualSize(context);

            var text = Content?.ToString();
            var font = Font ?? context.DefaultFont;
            return (Size)font.MeasureString(text ?? string.Empty);
        }
    }

}