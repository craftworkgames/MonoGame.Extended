using System;

namespace MonoGame.Extended.Gui.Controls
{
    public class Button : ContentControl
    {
        public Button()
        {
        }

        public event EventHandler Clicked;
        public event EventHandler PressedStateChanged;

        private bool _isPressed;
        public bool IsPressed
        {
            get => _isPressed;
            set
            {
                if (_isPressed != value)
                {
                    _isPressed = value;
                    PressedStyle?.ApplyIf(this, _isPressed);
                    PressedStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private ControlStyle _pressedStyle;
        public ControlStyle PressedStyle
        {
            get => _pressedStyle;
            set
            {
                if (_pressedStyle != value)
                {
                    _pressedStyle = value;
                    PressedStyle?.ApplyIf(this, _isPressed);
                }
            }
        }

        private bool _isPointerDown;

        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled)
            {
                _isPointerDown = true;
                IsPressed = true;
            }

            return base.OnPointerDown(context, args);
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            _isPointerDown = false;

            if (IsPressed)
            {
                IsPressed = false;

                if (BoundingRectangle.Contains(args.Position) && IsEnabled)
                    Click();
            }

            return base.OnPointerUp(context, args);
        }

        public override bool OnPointerEnter(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled && _isPointerDown)
                IsPressed = true;

            return base.OnPointerEnter(context, args);
        }

        public override bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsEnabled)
                IsPressed = false;

            return base.OnPointerLeave(context, args);
        }

        public void Click()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}