using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class CheckBox : ContentControl
    {
        public CheckBox()
        {
        }

        public event EventHandler CheckStateChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    CheckedStyle?.ApplyIf(this, _isChecked);
                    CheckStateChanged?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        private ControlStyle _checkedStyle;
        public ControlStyle CheckedStyle
        {
            get { return _checkedStyle; }
            set
            {
                if (_checkedStyle != value)
                {
                    _checkedStyle = value;
                    CheckedStyle?.ApplyIf(this, _isChecked);
                }
            }
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            if (IsFocused && BoundingRectangle.Contains(args.Position))
                IsChecked = !IsChecked;

            return base.OnPointerUp(context, args);
        }

        private Rectangle GetCheckRectangle()
        {
            var boundingRectangle = BoundingRectangle;

            if (BackgroundRegion != null)
                return new Rectangle(boundingRectangle.X, boundingRectangle.Y, BackgroundRegion.Width, BackgroundRegion.Height);

            return new Rectangle(boundingRectangle.X, boundingRectangle.Y, boundingRectangle.Height, boundingRectangle.Height);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, GetCheckRectangle(), BackgroundColor);
            else
            {
                renderer.DrawRectangle(GetCheckRectangle(), BorderColor, BorderThickness);

                if (IsChecked)
                {
                    var innerCheckRectangle = GetCheckRectangle();
                    innerCheckRectangle.Inflate(-4, -4);
                    renderer.FillRectangle(innerCheckRectangle, TextColor);
                }
            }
        }
    }
}
