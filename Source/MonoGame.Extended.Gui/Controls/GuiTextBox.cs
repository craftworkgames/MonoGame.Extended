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
        }

        public int SelectionStart { get; set; }

        public override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            SelectionStart = Text.Length;
            _isCaretVisible = true;
        }

        public override void OnKeyPressed(KeyboardEventArgs args)
        {
            base.OnKeyPressed(args);

            switch (args.Key)
            {
                case Keys.Back:
                    if (SelectionStart > 0 && Text.Length > 0)
                    {
                        SelectionStart--;
                        Text = Text.Remove(SelectionStart, 1);
                    }
                    break;
                case Keys.Delete:
                    if (SelectionStart < Text.Length)
                        Text = Text.Remove(SelectionStart, 1);
                    break;
                case Keys.Left:
                    if (SelectionStart > 0)
                        SelectionStart--;
                    break;
                case Keys.Right:
                    if (SelectionStart < Text.Length)
                        SelectionStart++;
                    break;
                default:
                    if (args.Character != null)
                    {
                        SelectionStart++;
                        Text += args.Character;
                    }
                    break;
            }

            _isCaretVisible = true;
        }

        private const float _caretBlinkRate = 0.53f;
        private float _nextCaretBlink = _caretBlinkRate;
        private bool _isCaretVisible = true;

        public override void Draw(IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(renderer, deltaSeconds);

            if (IsFocused)
            {
                if (_isCaretVisible)
                {
                    var font = Font ?? renderer.DefaultFont;
                    var textPosition = GetTextPosition(renderer);
                    var textRectangle = font.GetStringRectangle(Text.Substring(0, SelectionStart), textPosition);
                    textRectangle.X = textRectangle.Right;
                    textRectangle.Width = 1;
                    renderer.DrawRegion(null, textRectangle, TextColor);
                }

                _nextCaretBlink -= deltaSeconds;

                if (_nextCaretBlink <= 0)
                {
                    _isCaretVisible = !_isCaretVisible;
                    _nextCaretBlink = _caretBlinkRate;
                }
            }
        }
    }
}