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

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, GetCheckRectangle(), Color);
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

        //protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        //{
        //    var checkRectangle = GetCheckRectangle();
        //    textInfo.Position = BoundingRectangle.Location.ToVector2() +
        //            new Vector2(checkRectangle.Width + 5, checkRectangle.Height * 0.5f - textInfo.Font.LineHeight * 0.5f);

        //    base.DrawForeground(context, renderer, deltaSeconds, textInfo);
        //}
    }
}
