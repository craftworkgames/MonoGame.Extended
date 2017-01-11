using System;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public GuiButton()
        {
        }

        public GuiButton(TextureRegion2D textureRegion)
        {
            TextureRegion = textureRegion;
            Size = TextureRegion.Size;
        }

        public event EventHandler Clicked;

        private bool _isPressed;
        public bool IsPressed
        {
            get { return _isPressed; }
            set
            {
                _isPressed = value;
                PressedStyle?.ApplyIf(this, _isPressed);
            }
        }

        private GuiControlStyle _pressedStyle;
        public GuiControlStyle PressedStyle
        {
            get { return _pressedStyle; }
            set
            {
                _pressedStyle = value;
                PressedStyle?.ApplyIf(this, _isPressed);
            }
        }

        public override void OnMouseDown(MouseEventArgs args)
        {
            if (IsEnabled)
                IsPressed = true;

            base.OnMouseDown(args);
        }

        public override void OnMouseUp(MouseEventArgs args)
        {
            IsPressed = false;

            if(BoundingRectangle.Contains(args.Position) && IsEnabled)
                Clicked?.Invoke(this, EventArgs.Empty);

            base.OnMouseUp(args);
        }

        public override void OnMouseLeave(MouseEventArgs args)
        {
            if (IsEnabled)
                IsPressed = false;

            base.OnMouseLeave(args);
        }
    }
}