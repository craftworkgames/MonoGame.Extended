using System.Collections.Generic;
using System.Linq;
using System.Text;
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
       

        private const float _caretBlinkRate = 0.53f;
        private float _nextCaretBlink = _caretBlinkRate;
        private bool _isCaretVisible = true;

        private readonly List<StringBuilder> _lines = new List<StringBuilder>();
        public string Text
        {
            get => string.Concat(_lines.SelectMany(s => $"{s}\n"));
            set
            {
                _lines.Clear();

                var line = new StringBuilder();

                for (var i = 0; i < value.Length; i++)
                {
                    var c = value[i];

                    if (c == '\n')
                    {
                        _lines.Add(line);
                        line = new StringBuilder();
                    }
                    else if(c != '\r')
                    {
                        line.Append(c);
                    }
                }

                _lines.Add(line);
            }
        }

        public int CaretIndex => ColumnIndex * LineIndex + ColumnIndex;
        public int LineIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int TabStops { get; set; } = 4;

        public override IEnumerable<Control> Children => Enumerable.Empty<Control>();

        public string GetLineText(int lineIndex) => _lines[lineIndex].ToString();
        public int GetLineLength(int lineIndex) => _lines[lineIndex].Length;

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
                case Keys.Tab:
                    Tab();
                    break;
                case Keys.Back:
                    Backspace();
                    break;
                case Keys.Delete:
                    Delete();
                    break;
                case Keys.Left:
                    Left();
                    break;
                case Keys.Right:
                    Right();
                    break;
                case Keys.Up:
                    Up();
                    break;
                case Keys.Down:
                    Down();
                    break;
                case Keys.Home:
                    Home();
                    break;
                case Keys.End:
                    End();
                    break;
                case Keys.Enter:
                    Type('\n');
                    return true;
                default:
                    if (args.Character.HasValue)
                        Type(args.Character.Value);

                    break;
            }

            _isCaretVisible = true;
            return base.OnKeyPressed(context, args);
        }

        public void Type(char c)
        {
            switch (c)
            {
                case '\n':
                    var lineText = GetLineText(LineIndex);
                    var left = lineText.Substring(0, ColumnIndex);
                    var right = lineText.Substring(ColumnIndex);
                    _lines.Insert(LineIndex + 1, new StringBuilder(right));
                    _lines[LineIndex] = new StringBuilder(left);
                    LineIndex++;
                    Home();
                    break;
                case '\t':
                    Tab();
                    break;
                default:
                    _lines[LineIndex].Insert(ColumnIndex, c);
                    ColumnIndex++;
                    break;
            }
        }

        public void Backspace()
        {
            if (ColumnIndex == 0 && LineIndex > 0)
            {
                var topLineLength = GetLineLength(LineIndex - 1);

                if (RemoveLineBreak(LineIndex - 1))
                {
                    LineIndex--;
                    ColumnIndex = topLineLength;
                }
            }
            else if (Left())
            {
                RemoveCharacter(LineIndex, ColumnIndex);
            }
        }

        public void Delete()
        {
            var lineLength = GetLineLength(LineIndex);

            if (ColumnIndex == lineLength)
                RemoveLineBreak(LineIndex);
            else
                RemoveCharacter(LineIndex, ColumnIndex);
        }

        public void RemoveCharacter(int lineIndex, int columnIndex)
        {
            _lines[lineIndex].Remove(columnIndex, 1);
        }

        public bool RemoveLineBreak(int lineIndex)
        {
            if (lineIndex < _lines.Count - 1)
            {
                var topLine = _lines[lineIndex];
                var bottomLine = _lines[lineIndex + 1];
                _lines.RemoveAt(lineIndex + 1);
                topLine.Append(bottomLine);
                return true;
            }

            return false;
        }

        public bool Home()
        {
            ColumnIndex = 0;
            return true;
        }

        public bool End()
        {
            ColumnIndex = GetLineLength(LineIndex);
            return true;
        }

        public bool Up()
        {
            if (LineIndex > 0)
            {
                LineIndex--;

                if (ColumnIndex > GetLineLength(LineIndex))
                    ColumnIndex = GetLineLength(LineIndex);
                
                return true;
            }

            return false;
        }

        public bool Down()
        {
            if (LineIndex < _lines.Count - 1)
            {
                LineIndex++;

                if (ColumnIndex > GetLineLength(LineIndex))
                    ColumnIndex = GetLineLength(LineIndex);

                return true;
            }

            return false;
        }

        public bool Left()
        {
            if (ColumnIndex == 0)
            {
                if (LineIndex == 0)
                    return false;

                LineIndex--;
                ColumnIndex = GetLineLength(LineIndex);
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

        public bool Tab()
        {
            var spaces = TabStops - ColumnIndex % TabStops;

            for (var s = 0; s < spaces; s++)
                Type(' ');

            return spaces > 0;
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