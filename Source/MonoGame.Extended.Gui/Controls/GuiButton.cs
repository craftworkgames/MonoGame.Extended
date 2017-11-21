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

        public override bool OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled)
            {
                _isPointerDown = true;
                IsPressed = true;
            }
            return base.OnPointerDown(context, args);
        }

        public override bool OnPointerUp(IGuiContext context, GuiPointerEventArgs args)
        {
            _isPointerDown = false;

            if (IsPressed)
            {
                IsPressed = false;

                if (BoundingRectangle.Contains(args.Position) && IsEnabled)
                    TriggerClicked();
            }
            return base.OnPointerUp(context, args);
        }

        public override bool OnPointerEnter(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled && _isPointerDown)
                IsPressed = true;

            return base.OnPointerEnter(context, args);
        }

        public override bool OnPointerLeave(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsEnabled)
                IsPressed = false;
            return base.OnPointerLeave(context, args);
        }

        public void TriggerClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
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