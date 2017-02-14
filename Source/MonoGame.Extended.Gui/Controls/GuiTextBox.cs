using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiTextBox : GuiControl
    {
        public GuiTextBox()
            : this(textureRegion: null, text: string.Empty)
        {
        }

        public GuiTextBox(TextureRegion2D textureRegion)
            : this(textureRegion: textureRegion, text: string.Empty)
        {
        }

        public GuiTextBox(TextureRegion2D textureRegion, string text)
            : base(textureRegion)
        {
            Text = text;
            IsKeyboardFocused = false;
        }

        public int SelectionStart { get; set; }
        public bool IsKeyboardFocused { get; set; }

        public override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            SelectionStart = Text.Length;
            IsKeyboardFocused = true;
        }

        public override void OnKeyPressed(KeyboardEventArgs args)
        {
            base.OnKeyPressed(args);

            if (args.Key == Keys.Back)
            {
                if (SelectionStart > 0 && Text.Length > 0)
                {
                    SelectionStart--;
                    Text = Text.Remove(SelectionStart, 1);
                }
            }
            else if(args.Key == Keys.Delete)
            {
                if (SelectionStart < Text.Length)
                    Text = Text.Remove(SelectionStart, 1);
            }
            else if (args.Key == Keys.Left)
            {
                if (SelectionStart > 0)
                    SelectionStart--;
            }
            else if (args.Key == Keys.Right)
            {
                if (SelectionStart < Text.Length)
                    SelectionStart++;
            }
            else if (args.Character != null)
            {
                SelectionStart++;
                Text += args.Character;
            }

            _isCursorVisible = true;
        }

        private const float _blinkRate = 0.53f;
        private float _nextBlink = _blinkRate;
        private bool _isCursorVisible = true;

        public override void Draw(IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(renderer, deltaSeconds);

            if (IsKeyboardFocused)
            {
                if (_isCursorVisible)
                {
                    var font = Font ?? renderer.DefaultFont;
                    var textPosition = GetTextPosition(renderer);
                    var textRectangle = font.GetStringRectangle(Text.Substring(0, SelectionStart), textPosition);
                    textRectangle.X = textRectangle.Right;
                    textRectangle.Width = 1;
                    renderer.DrawRegion(null, textRectangle, TextColor);
                }

                _nextBlink -= deltaSeconds;

                if (_nextBlink <= 0)
                {
                    _isCursorVisible = !_isCursorVisible;
                    _nextBlink = _blinkRate;
                }
            }
        }
    }
}