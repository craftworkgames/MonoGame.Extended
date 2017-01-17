using System;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiProgressBar : GuiControl
    {
        public GuiProgressBar()
        {
        }

        public GuiProgressBar(TextureRegion2D textureRegion)
        {
            TextureRegion = textureRegion;
            Size = textureRegion.Size;
        }

        public float Progress { get; set; }
    }

    public class GuiCheckbox : GuiControl
    {
        public GuiCheckbox()
        {
        }

        public GuiCheckbox(TextureRegion2D textureRegion)
        {
            TextureRegion = textureRegion;
            Size = TextureRegion.Size;
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
                }
            }
        }

        private GuiControlStyle _checkedStyle;
        public GuiControlStyle CheckedStyle
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

        public override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            if (IsFocused && BoundingRectangle.Contains(args.Position))
                IsChecked = !IsChecked;
        }
    }
}