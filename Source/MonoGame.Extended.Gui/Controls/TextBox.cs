using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class TextContainer
    {
        public TextContainer()
        {
            _stringBuilder = new StringBuilder();
        }

        private readonly StringBuilder _stringBuilder;

        public int LineIndex { get; set; }
        public int LineCount { get; private set; }
        public int ColumnIndex { get; set; }
        public int CaretIndex => ColumnIndex * LineCount + ColumnIndex;

        public void Append(char c)
        {
            if (c == '\n')
            {
                LineIndex++;
                ColumnIndex = 0;
            }

            _stringBuilder.Append(c);
        }
    }

    public sealed class TextBox : Control
    {
        public TextBox()
            : this(null)
        {
        }

        public TextBox(string text)
        {
            Text = text ?? string.Empty;
            HorizontalTextAlignment = HorizontalAlignment.Left;
            VerticalTextAlignment = VerticalAlignment.Top;
        }

        private int _caretIndex;
        public int CaretIndex
        {
            get => _caretIndex;
            set
            {
                if (_caretIndex != value)
                {
                    _caretIndex = value;
                    CaretIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int LineIndex => GetLineIndex(CaretIndex);
        public int LineCount => Text.Count(c => c == '\n');
        public char? PasswordCharacter { get; set; }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnTextChanged();
                }
            }
        }

        private void OnTextChanged()
        {
            if (!string.IsNullOrEmpty(Text) && CaretIndex > Text.Length)
                CaretIndex = Text.Length;

            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TextChanged;
        public event EventHandler CaretIndexChanged;

        public override IEnumerable<Control> Children { get; } = Enumerable.Empty<Control>();

        public int GetLineIndex(int characterIndex)
        {
            var lineIndex = 0;

            for (var c = 0; c < characterIndex; c++)
            {
                if (_text[c] == '\n')
                    lineIndex++;
            }

            return lineIndex;
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var font = Font ?? context.DefaultFont;
            var stringSize = (Size)font.MeasureString(Text ?? string.Empty);
            return new Size(stringSize.Width, stringSize.Height < font.LineHeight ? font.LineHeight : stringSize.Height);
        }
        
        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            CaretIndex = FindNearestGlyphIndex(context, args.Position);
            _isCaretVisible = true;

            return base.OnPointerDown(context, args);
        }

        private int FindNearestGlyphIndex(IGuiContext context, Point position)
        {
            var font = Font ?? context.DefaultFont;
            var textInfo = GetTextInfo(context, Text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);
            var i = 0;

            foreach (var glyph in font.GetGlyphs(textInfo.Text, textInfo.Position))
            {
                var fontRegionWidth = glyph.FontRegion?.Width ?? 0;
                var glyphMiddle = (int)(glyph.Position.X + fontRegionWidth * 0.5f);

                if (position.X >= glyphMiddle)
                {
                    i++;
                    continue;
                }

                return i;
            }

            return i;
        }

        public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case Keys.Tab:
                    return true;
                case Keys.Back:
                    if (Text.Length > 0 && CaretIndex > 0)
                    {
                        CaretIndex--;
                        Text = Text.Remove(CaretIndex, 1);
                    }
                    break;
                case Keys.Delete:
                    if (CaretIndex < Text.Length)
                        Text = Text.Remove(CaretIndex, 1);

                    break;
                case Keys.Left:
                    if (CaretIndex > 0)
                        CaretIndex--;

                    break;
                case Keys.Right:
                    if (CaretIndex < Text.Length)
                        CaretIndex++;

                    break;
                case Keys.Home:
                    CaretIndex = 0;
                    break;
                case Keys.End:
                    CaretIndex = Text.Length;
                    break;
                case Keys.Enter:
                    Text = Text.Insert(CaretIndex, Environment.NewLine);
                    CaretIndex++;
                    return true;
                default:
                    if (args.Character != null)
                    {
                        Text = Text.Insert(CaretIndex, args.Character.ToString());
                        CaretIndex++;
                    }
                    break;
            }

            _isCaretVisible = true;
            return base.OnKeyPressed(context, args);
        }

        private const float _caretBlinkRate = 0.53f;
        private float _nextCaretBlink = _caretBlinkRate;
        private bool _isCaretVisible = true;

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            var text = PasswordCharacter.HasValue ? new string(PasswordCharacter.Value, Text.Length) : Text;
            var textInfo = GetTextInfo(context, text, ContentRectangle, HorizontalTextAlignment, VerticalTextAlignment);

            if (!string.IsNullOrWhiteSpace(textInfo.Text))
                renderer.DrawText(textInfo.Font, textInfo.Text, textInfo.Position + TextOffset, textInfo.Color, textInfo.ClippingRectangle);

            if (IsFocused)
            {
                var caretRectangle = GetCaretRectangle(textInfo, CaretIndex);

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
        
        private Rectangle GetCaretRectangle(TextInfo textInfo, int index)
        {
            var caretRectangle = textInfo.Font.GetStringRectangle(textInfo.Text.Substring(0, index), textInfo.Position);

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
            return (Rectangle) caretRectangle;
        }
    }
}