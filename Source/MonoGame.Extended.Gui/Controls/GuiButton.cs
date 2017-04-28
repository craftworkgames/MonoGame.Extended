using System;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public GuiButton()
            : this(null)
        {
        }

        public GuiButton(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var size = base.CalculateDesiredSize(context, availableSize);
            return size;
        }

        public event EventHandler Clicked;
        public event EventHandler PressedStateChanged;

        private bool _isPressed;
        public bool IsPressed
        {
            get { return _isPressed; }
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

        private GuiControlStyle _pressedStyle;
        public GuiControlStyle PressedStyle
        {
            get { return _pressedStyle; }
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

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);

            if (IsEnabled)
            {
                _isPointerDown = true;
                IsPressed = true;
            }
        }

        public override void OnPointerUp(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerUp(context, args);

            _isPointerDown = false;

            if (IsPressed)
            {
                IsPressed = false;

                if (BoundingRectangle.Contains(args.Position) && IsEnabled)
                    Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void OnPointerEnter(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerEnter(context, args);

            if (IsEnabled && _isPointerDown)
                IsPressed = true;
        }

        public override void OnPointerLeave(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerLeave(context, args);

            if (IsEnabled)
                IsPressed = false;
        }
    }
}