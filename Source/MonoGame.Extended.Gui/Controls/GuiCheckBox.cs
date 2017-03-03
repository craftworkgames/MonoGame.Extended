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

        public GuiCheckBox(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
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

        public override void OnPointerUp(GuiPointerEventArgs args)
        {
            base.OnPointerUp(args);

            if (IsFocused && BoundingRectangle.Contains(args.Position))
                IsChecked = !IsChecked;
        }

        protected override void DrawBackground(IGuiRenderer renderer, float deltaSeconds)
        {
            var boundingRectangle = BoundingRectangle;
            var checkRectangle = new Rectangle(boundingRectangle.Location, BackgroundRegion.Size);

            renderer.DrawRegion(BackgroundRegion, checkRectangle, Color);
        }

        protected override void DrawText(IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            textInfo.Position = BoundingRectangle.Location.ToVector2() +
                    new Vector2(BackgroundRegion.Width + 5, BackgroundRegion.Height * 0.5f - textInfo.Font.LineHeight * 0.5f);

            base.DrawText(renderer, deltaSeconds, textInfo);
        }
    }
}
