using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
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

        public TextureRegion2D HotIcon { get; set; }
        public Point HotIconOffset { get; set; }
        public string Text { get { return (string)Content; } set { Content = value; } }

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
            //if (IsEnabled)
            //    IsPressed = false;

            return base.OnPointerLeave(context, args);
        }

        public void Click()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            if (HotIcon != null)
                DrawHotIcon(context, renderer, deltaSeconds, this.ClippingRectangle);
        }

        protected virtual void DrawHotIcon(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, Rectangle? clippingRectangle)
        {
            var rect = clippingRectangle != null ? clippingRectangle.Value.Clip(BoundingRectangle) : BoundingRectangle;
            var minSize = Math.Min(rect.Width, rect.Height);
            var hotIconSize = new Size((int)HotIcon.Size.Width, (int)HotIcon.Size.Height);
            var imageSize = hotIconSize == Size.Empty ? new Size(minSize, minSize) : hotIconSize;
            var destinationRectangle = LayoutHelper.AlignRectangle(HorizontalAlignment.Centre, VerticalAlignment.Centre, imageSize, rect);
            destinationRectangle.Location = new Point(destinationRectangle.Location.X + HotIconOffset.X, destinationRectangle.Location.Y + HotIconOffset.Y);
            renderer.DrawRegion(HotIcon, destinationRectangle, Color.White, clippingRectangle);
        }
    }
}