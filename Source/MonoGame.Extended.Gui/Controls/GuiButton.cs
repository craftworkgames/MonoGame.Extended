using System;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        private bool _isPressed;

        public GuiButton()
        {
        }

        public bool IsPressed
        {
            get { return _isPressed; }
            private set
            {
                if (_isPressed != value)
                {
                    _isPressed = value;

                    if (_isPressed)
                        PressedStyle?.Apply(this);
                    else
                        PressedStyle?.Revert(this);
                }
            }
        }

        public GuiControlStyle PressedStyle { get; set; }

        public event EventHandler<MouseEventArgs> Click;

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (IsEnabled)
                IsPressed = true;

            base.OnMouseDown(sender, args);
        }

        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (IsPressed && Contains(args.Position))
                Click?.Invoke(this, args);

            IsPressed = false;
            base.OnMouseUp(sender, args);
        }

        public override void OnMouseLeave(object sender, MouseEventArgs args)
        {
            base.OnMouseLeave(sender, args);

            if (IsEnabled)
                IsPressed = false;
        }
    }
}