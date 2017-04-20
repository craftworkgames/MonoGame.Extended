using System.Linq;
using Microsoft.Xna.Framework;
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
        public char? PasswordCharacter { get; set; }

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);
            
            SelectionStart = FindNearestGlyphIndex(context, args.Position);
            _isCaretVisible = true;
        }

        private int FindNearestGlyphIndex(IGuiContext context, Point position)
        {
            var font = Font ?? context.DefaultFont;
            var textInfo = GetTextInfo(context, Text, BoundingRectangle, HorizontalAlignment.Centre, VerticalAlignment.Centre);
            var glyphs = font.GetGlyphs(textInfo.Text, textInfo.Position).ToArray();

            for (var i = 0; i < glyphs.Length; i++)
            {
                var glyph = glyphs[i];
                var glyphMiddle = (int)(glyph.Position.X + glyph.FontRegion.Width * 0.5f);

                if (position.X >= glyphMiddle)
                    continue;

                return i;
            }

            return glyphs.Length;
        }

        public override void OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            base.OnKeyPressed(context, args);

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

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            if (PasswordCharacter.HasValue)
                textInfo = GetTextInfo(context, new string(PasswordCharacter.Value, textInfo.Text.Length), BoundingRectangle, HorizontalAlignment.Centre, VerticalAlignment.Centre);

            var caretRectangle = textInfo.Font.GetStringRectangle(textInfo.Text.Substring(0, SelectionStart), textInfo.Position);

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

            base.DrawForeground(context, renderer, deltaSeconds, textInfo);

            if (IsFocused)
            {
                if (_isCaretVisible)
                    renderer.DrawRectangle((Rectangle)caretRectangle, TextColor);

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