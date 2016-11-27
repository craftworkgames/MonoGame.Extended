using System;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiToggleButton : GuiControl
    {
        private bool _isChecked;

        public GuiToggleButton()
        {
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;

                    if (_isChecked)
                        CheckedStyle?.Apply(this);
                    else
                        CheckedStyle?.Revert(this);
                }
            }
        }

        public bool IsPressed { get; private set; }
        public GuiControlStyle CheckedStyle { get; set; }
        public event EventHandler<EventArgs> CheckStateChanged;

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (IsEnabled)
                IsPressed = true;

            base.OnMouseDown(sender, args);
        }

        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (IsPressed && Contains(args.Position))
            {
                IsChecked = !IsChecked;
                CheckStateChanged?.Invoke(this, args);
            }

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