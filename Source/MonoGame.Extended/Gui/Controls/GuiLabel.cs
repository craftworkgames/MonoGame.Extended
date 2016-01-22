using MonoGame.Extended.Gui.Drawables;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabel : GuiControl
    {
        public GuiLabel(GuiLabelStyle style)
            : this(style, string.Empty)
        {
        }

        public GuiLabel(GuiLabelStyle style, string text)
        {
            Style = style;
            Text = text;
        }

        private GuiTextDrawable _drawable;
        private bool _propertyChanged;

        private GuiLabelStyle _style;
        public GuiLabelStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                _propertyChanged = true;
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _propertyChanged = true;
            }
        }

        protected override IGuiDrawable GetCurrentDrawable()
        {
            if (_propertyChanged)
            {
                _drawable = new GuiTextDrawable(Style.Font, Text, Style.Color);
                _propertyChanged = false;
            }

            return _drawable;
        }
    }
}