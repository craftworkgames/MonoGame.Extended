using System;

namespace MonoGame.Extended.Gui.Controls
{
    public class ToggleButton : Button
    {
        public ToggleButton()
        {
        }

        public event EventHandler CheckedStateChanged;

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
                    CheckedStateChanged?.Invoke(this, EventArgs.Empty);
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

        private ControlStyle _checkedHoverStyle;
        public ControlStyle CheckedHoverStyle
        {
            get { return _checkedHoverStyle; }
            set
            {
                if (_checkedHoverStyle != value)
                {
                    _checkedHoverStyle = value;
                    CheckedHoverStyle?.ApplyIf(this, _isChecked && IsHovered);
                }
            }
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            base.OnPointerUp(context, args);

            if (BoundingRectangle.Contains(args.Position))
            {
                HoverStyle?.Revert(this);
                CheckedHoverStyle?.Revert(this);

                IsChecked = !IsChecked;

                if (IsChecked)
                    CheckedHoverStyle?.Apply(this);
                else
                    HoverStyle?.Apply(this);
            }

            return true;
        }

        public override bool OnPointerEnter(IGuiContext context, PointerEventArgs args)
        {
            if (IsChecked)
            {
                CheckedHoverStyle?.Apply(this);
                return true;
            }

            return base.OnPointerEnter(context, args);
        }

        public override bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (IsChecked)
            {
                CheckedHoverStyle?.Revert(this);
                return true;
            }

            return base.OnPointerLeave(context, args);
        }
    }
}