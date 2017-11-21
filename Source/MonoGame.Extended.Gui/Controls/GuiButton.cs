using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public GuiButton()
            : base(null)
        {
        }

        public GuiButton(GuiSkin skin)
            : base(skin)
        {
        }

        private Point _iconPosition;

        public Color IconColor { get; set; } = Color.White;

        private TextureRegion2D _iconRegion;
        public TextureRegion2D IconRegion
        {
            get { return _iconRegion; }
            set
            {
                if (_iconRegion != value)
                {
                    _iconRegion = value;
                    RecalculateIconPosition();
                }
            }
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

        protected override void OnSizeChanged()
        {
            if (IconRegion != null)
                RecalculateIconPosition();
        }

        private void RecalculateIconPosition()
        {
            var x = (BoundingRectangle.Width - IconRegion.Width) / 2;
            var y = (BoundingRectangle.Height - IconRegion.Height) / 2;
            _iconPosition = new Point(x, y);
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            base.DrawForeground(context, renderer, deltaSeconds, textInfo);

            if (IconRegion != null)
            {
                renderer.DrawRegion(IconRegion, new Rectangle(BoundingRectangle.Location + _iconPosition + Offset.ToPoint(), IconRegion.Bounds.Size), IconColor);
            }
        }
    }
}