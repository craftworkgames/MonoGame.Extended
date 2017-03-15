using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiTextBox : GuiControl
    {
        public GuiTextBox()
            : this(backgroundRegion: null, text: string.Empty)
        {
        }

        public GuiTextBox(TextureRegion2D backgroundRegion)
            : this(backgroundRegion: backgroundRegion, text: string.Empty)
        {
        }

        public GuiTextBox(TextureRegion2D backgroundRegion, string text)
            : base(backgroundRegion)
        {
            Text = text;
        }

        public int SelectionStart { get; set; }

        public override void OnPointerDown(GuiPointerEventArgs args)
        {
            base.OnPointerDown(args);

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

        protected override void DrawText(IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            var caretRectangle = textInfo.Font.GetStringRectangle(Text.Substring(0, SelectionStart), textInfo.Position);

            // TODO: Finish the caret position stuff when it's outside the clipping rectangle
            if (caretRectangle.Right > ClippingRectangle.Right)
            {
                var textOffset = caretRectangle.Right - ClippingRectangle.Right;
                textInfo.Position.X -= textOffset;
            }

            caretRectangle.X = caretRectangle.Right < ClippingRectangle.Right ? caretRectangle.Right : ClippingRectangle.Right;
            caretRectangle.Width = 1;

            base.DrawText(renderer, deltaSeconds, textInfo);

            if (IsFocused)
            {
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
    }
}