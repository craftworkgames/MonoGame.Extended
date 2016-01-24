using MonoGame.Extended.Gui.Drawables;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiCheckBox : GuiControl
    {
        private readonly GuiCheckBoxStyle _style;

        public GuiCheckBox(GuiCheckBoxStyle style)
        {
            _style = style;
        }

        public bool IsChecked { get; set; }

        private bool _isMouseDown;

        public override void OnMouseLeave(object sender, MouseEventArgs args)
        {
            _isMouseDown = false;
            base.OnMouseLeave(sender, args);
        }

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            _isMouseDown = true;
            base.OnMouseDown(sender, args);
        }
        
        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (_isMouseDown)
                IsChecked = !IsChecked;

            _isMouseDown = false;
            base.OnMouseUp(sender, args);
        }

        protected override IGuiDrawable GetCurrentDrawable()
        {
            return IsChecked ? _style.CheckedOn : _style.CheckedOff;
        }
    }
}