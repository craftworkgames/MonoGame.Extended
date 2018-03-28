using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class Box : Control
    {
        public override IEnumerable<Control> Children { get; } = Enumerable.Empty<Control>();

        public override Size GetContentSize(IGuiContext context)
        {
            return new Size(Width, Height);
        }

        public Color FillColor { get; set; } = Color.White;

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);
            renderer.FillRectangle(ContentRectangle, FillColor);
        }
    }

    public class CheckBox : CompositeControl
    {
        public CheckBox()
        {
            _contentLabel = new Label();
            _checkLabel = new Box {Width = 20, Height = 20};

            _toggleButton = new ToggleButton
            {
                Margin = 0,
                Padding = 0,
                BackgroundColor = Color.Transparent,
                BorderThickness = 0,
                HoverStyle = null,
                CheckedStyle = null,
                PressedStyle = null,
                Content = new StackPanel
                {
                    Margin = 0,
                    Orientation = Orientation.Horizontal,
                    Items =
                    {
                        _checkLabel,
                        _contentLabel
                    }
                }
            };

            _toggleButton.CheckedStateChanged += (sender, args) => OnIsCheckedChanged();
            Template = _toggleButton;
            OnIsCheckedChanged();
        }
        
        private readonly Label _contentLabel;
        private readonly ToggleButton _toggleButton;
        private readonly Box _checkLabel;

        protected override Control Template { get; }

        public override object Content
        {
            get { return _contentLabel.Content; }
            set { _contentLabel.Content = value; }
        }

        public bool IsChecked
        {
            get { return _toggleButton.IsChecked; }
            set
            {
                _toggleButton.IsChecked = value;
                OnIsCheckedChanged();
            }
        }

        private void OnIsCheckedChanged()
        {
            _checkLabel.FillColor = IsChecked ? Color.White : Color.Transparent;
        }
    }
}