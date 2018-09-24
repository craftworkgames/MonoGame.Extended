using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class TextBox2 : Control
    {
        public TextBox2()
            : this(null)
        {
        }

        public TextBox2(string text)
        {
            Text = text ?? string.Empty;
            HorizontalTextAlignment = HorizontalAlignment.Left;
            VerticalTextAlignment = VerticalAlignment.Top;
        }
        
        private struct TextLine
        {
            public int StartIndex;
            public int Length;
        }

        private readonly List<TextLine> _lines = new List<TextLine>();
        private const float _caretBlinkRate = 0.53f;
        private float _nextCaretBlink = _caretBlinkRate;
        private bool _isCaretVisible = true;

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateLineInfo();
            }
        }

        public int CaretIndex => ColumnIndex * LineIndex + ColumnIndex;
        public int LineIndex { get; set; }
        public int ColumnIndex { get; set; }

        private void UpdateLineInfo()
        {
            _lines.Clear();
            var line = new TextLine();

            for (var i = 0; i < _text.Length; i++)
            {
                var c = _text[i];

                if (c == '\n')
                {
                    _lines.Add(line);
                    line = new TextLine {StartIndex = i + 1};
                }
                else if (c != '\r')
                {
                    line.Length++;
                }
            }

            _lines.Add(line);
        }

        public override IEnumerable<Control> Children => Enumerable.Empty<Control>();

        public string GetLineText(int lineIndex)
        {
            var line = _lines[lineIndex];
            return Text.Substring(line.StartIndex, line.Length);
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var font = Font ?? context.DefaultFont;
            var stringSize = (Size)font.MeasureString(Text ?? string.Empty);
            return new Size(stringSize.Width, stringSize.Height < font.LineHeight ? font.LineHeight : stringSize.Height);
        }

        public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                //case Keys.Tab:
                //    return true;
                //case Keys.Back:
                //    _textContainer.Backspace();
                //    break;
                //case Keys.Delete:
                //    _textContainer.Delete();
                //    break;
                case Keys.Left:
                    Left();
                    break;
                case Keys.Right:
                    Right();
                    break;
                //case Keys.Home:
                //    _textContainer.Home();
                //    break;
                //case Keys.End:
                //    _textContainer.End();
                //    break;
                //case Keys.Enter:
                //    _textContainer.Type('\n');
                //    return true;
                //default:
                //    if (args.Character.HasValue)
                //        _textContainer.Type(args.Character.Value);

                //    break;
            }

            _isCaretVisible = true;
            return base.OnKeyPressed(context, args);
        }

        public bool Left()
        {
            if (ColumnIndex == 0)
            {
                if (LineIndex == 0)
                    return false;

                LineIndex--;
                ColumnIndex = _lines[LineIndex].Length;
            }
            else
            {
                ColumnIndex--;
            }

            return true;
        }

        public bool Right()
        {
            if (ColumnIndex == _lines[LineIndex].Length)
            {
                if (LineIndex == _lines.Count - 1)
                    return false;

                LineIndex++;
                ColumnIndex = 0;
            }
            else
            {
                ColumnIndex++;
            }

            return true;
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var text = Text;
            var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);

            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);

            if (IsFocused)
            {
                var caretRectangle = GetCaretRectangle(textInfo);

                if (_isCaretVisible)
                    renderer.DrawRectangle(caretRectangle, TextColor);

                _nextCaretBlink -= deltaSeconds;

                if (_nextCaretBlink <= 0)
                {
                    _isCaretVisible = !_isCaretVisible;
                    _nextCaretBlink = _caretBlinkRate;
                }
            }
        }

        private Rectangle GetCaretRectangle(TextInfo textInfo)
        {
            var font = textInfo.Font;
            var text = GetLineText(LineIndex);
            var offset = new Vector2(0, font.LineHeight * LineIndex);
            var caretRectangle = font.GetStringRectangle(text.Substring(0, ColumnIndex), textInfo.Position + offset);

            // TODO: Finish the caret position stuff when it's outside the clipping rectangle
            if (caretRectangle.Right > ClippingRectangle.Right)
                textInfo.Position.X -= caretRectangle.Right - ClippingRectangle.Right;

            caretRectangle.X = caretRectangle.Right < ClippingRectangle.Right ? caretRectangle.Right : ClippingRectangle.Right;
            caretRectangle.Width = 1;

            if (caretRectangle.Left < ClippingRectangle.Left)
            {
                textInfo.Position.X += ClippingRectangle.Left - caretRectangle.Left;
                caretRectangle.X = ClippingRectangle.Left;
            }

            return (Rectangle)caretRectangle;
        }
    }
}