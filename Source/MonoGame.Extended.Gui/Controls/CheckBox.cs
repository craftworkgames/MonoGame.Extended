using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class CheckBox : StackPanel
    {
        public CheckBox()
        {
            _contentLabel = new Label() { BackgroundColor = Color.Transparent };
            _checkLabel = new Box { Width = 20, Height = 20, BackgroundColor = Color.Transparent, BorderColor = Color.Black, BorderThickness = 2 };

            Margin = 0;
            Orientation = Orientation.Horizontal;
            BackgroundColor = Color.Transparent;
            Items.Add(_checkLabel);
            Items.Add(_contentLabel);
            _contentLabel.PointerUp += (sender, args) => OnPointerUp((IGuiContext)sender, args);
            _checkLabel.PointerUp += (sender, args) => OnPointerUp((IGuiContext)sender, args);

            OnIsCheckedChanged();
        }

        public event EventHandler CheckedStateChanged;

        private readonly Label _contentLabel;
        private readonly Box _checkLabel;
        private bool _isChecked;

        public string Text
        {
            get { return _contentLabel.Text; }
            set { _contentLabel.Text = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnIsCheckedChanged();
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

            if (BoundingRectangle.Contains(args.Position) ||_contentLabel.BoundingRectangle.Contains(args.Position)||_checkLabel.BoundingRectangle.Contains(args.Position))
            {
                HoverStyle?.Revert(this);

                IsChecked = !IsChecked;

                if (IsChecked)
                    CheckedHoverStyle?.Apply(this);
                else
                    HoverStyle?.Apply(this);
            }

            return true;
        }

        private void OnIsCheckedChanged()
        {
            _checkLabel.FillColor = IsChecked ? Color.White : Color.Transparent;
            CheckedStateChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}