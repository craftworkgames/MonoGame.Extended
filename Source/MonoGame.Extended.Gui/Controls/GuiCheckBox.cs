using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiCheckBox : GuiControl
    {
        public GuiCheckBox()
        {
        }

        public GuiCheckBox(TextureRegion2D textureRegion)
            : base(textureRegion)
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

        public override void Draw(IGuiRenderer renderer, float deltaSeconds)
        {
            var boundingRectangle = BoundingRectangle.ToRectangle();
            var checkRectangle = new Rectangle(boundingRectangle.Location, TextureRegion.Size);
            renderer.DrawRegion(TextureRegion, checkRectangle, Color);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                var font = Font ?? renderer.DefaultFont;
                var textPosition = BoundingRectangle.Location +
                                   new Vector2(TextureRegion.Width + 5,
                                       TextureRegion.Height * 0.5f - font.LineHeight * 0.5f);
                renderer.DrawText(Font, Text, textPosition, TextColor);
            }
        }
    }
}
