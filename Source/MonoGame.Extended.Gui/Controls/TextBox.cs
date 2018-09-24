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
            _lines = new List<StringBuilder>();
        }

        private readonly List<StringBuilder> _lines;

        public int LineIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int LineCount { get; private set; } = 1;
        public int CaretIndex
        {
            get => ColumnIndex * LineIndex + ColumnIndex;
            //set
            //{
            //    LineIndex = LineIndex / value;
            //    ColumnIndex = (LineIndex + 1) % value;
            //}
        }

        public string Text
        {
            get => string.Concat(_lines.SelectMany(s => $"{s}\n"));
            set
            {
                _lines.Clear();

                foreach (var s in value.Split('\n'))
                    _lines.Add(new StringBuilder(s.Trim('\r')));
            }
        }

        private StringBuilder CurrentLine() => _lines[LineIndex];

        //public void Type(string value)
        //{
        //    for (var i = 0; i < value.Length; i++)
        //        Type(value[i]);
        //}

        public void Type(char c)
        {
            CurrentLine().Insert(ColumnIndex, c);

            if (c == '\n')
            {
                LineIndex++;
                LineCount++;
                ColumnIndex = 0;
            }
            else
            {
                ColumnIndex++;
            }
        }

        private void RemoveCharacterAt(int index)
        {
            var c = CurrentLine()[index];

            if (c == '\n')
                LineCount--;

            CurrentLine().Remove(index, 1);
        }

        public void Backspace()
        {
            if (Left())
                RemoveCharacterAt(ColumnIndex);
        }

        public void Delete()
        {
            if (ColumnIndex < CurrentLine().Length)
                RemoveCharacterAt(ColumnIndex);
        }

        public bool Left()
        {
            if (ColumnIndex == 0)
                return false;

            if (ColumnIndex == 0)
                LineIndex--;
            else
                ColumnIndex--;

            return true;
        }

        public bool Right()
        {
            if (ColumnIndex >= CurrentLine().Length)
                return false;

            if (CurrentLine()[ColumnIndex] == '\n')
                LineIndex++;
            else
                ColumnIndex++;

            return true;
        }

        public void Home()
        {
            ColumnIndex = 0;
        }

        public void End()
        {

        }
    }

    public sealed class TextBox : Control
    {
        private readonly TextContainer _textContainer = new TextContainer();

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

        //private int _caretIndex;
        public int CaretIndex
        {
            get => _textContainer.CaretIndex;
            //private set
            //{
            //    if (_textContainer.CaretIndex != value)
            //    {
            //        _textContainer.CaretIndex = value;
            //        CaretIndexChanged?.Invoke(this, EventArgs.Empty);
            //    }
            //}
        }

        public int LineIndex => _textContainer.LineIndex;
        public int LineCount => _textContainer.LineCount;
        public char? PasswordCharacter { get; set; }

        public string Text
        {
            get => _textContainer.Text;
            set
            {
                if (_textContainer.Text != value)
                {
                    _textContainer.Text = value;
                    OnTextChanged();
                }
            }
        }

        private void OnTextChanged()
        {
            //if (!string.IsNullOrEmpty(Text) && CaretIndex > Text.Length)
            //    CaretIndex = Text.Length;

            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TextChanged;
        public event EventHandler CaretIndexChanged;

        public override IEnumerable<Control> Children { get; } = Enumerable.Empty<Control>();

        //public int GetLineIndex(int characterIndex)
        //{
        //    var lineIndex = 0;

        //    for (var c = 0; c < characterIndex; c++)
        //    {
        //        if (_text[c] == '\n')
        //            lineIndex++;
        //    }

        //    return lineIndex;
        //}

        public override Size GetContentSize(IGuiContext context)
        {
            var font = Font ?? context.DefaultFont;
            var stringSize = (Size)font.MeasureString(Text ?? string.Empty);
            return new Size(stringSize.Width, stringSize.Height < font.LineHeight ? font.LineHeight : stringSize.Height);
        }
        
        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            _textContainer.ColumnIndex = FindNearestGlyphIndex(context, args.Position);
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
                    _textContainer.Backspace();
                    break;
                case Keys.Delete:
                    _textContainer.Delete();
                    break;
                case Keys.Left:
                    _textContainer.Left();
                    break;
                case Keys.Right:
                    _textContainer.Right();
                    break;
                case Keys.Home:
                    _textContainer.Home();
                    break;
                case Keys.End:
                    _textContainer.End();
                    break;
                case Keys.Enter:
                    _textContainer.Type('\n');
                    return true;
                default:
                    if (args.Character.HasValue)
                        _textContainer.Type(args.Character.Value);

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