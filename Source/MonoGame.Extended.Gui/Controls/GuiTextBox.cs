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

        public bool IsKeyboardFocused { get; set; }

        public override void OnKeyTyped(KeyboardEventArgs args)
        {
            base.OnKeyTyped(args);

            if (args.Key == Keys.Back && Text.Length > 0)
                Text = Text.Substring(0, Text.Length - 1);
            else
                Text += args.Character;
        }
    }
}