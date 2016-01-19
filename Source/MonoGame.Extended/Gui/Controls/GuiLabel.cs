using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Styles;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabelStyle : GuiControlStyle<GuiLabel>
    {
        private readonly BitmapFont _font;

        public GuiLabelStyle(BitmapFont font)
        {
            _font = font;
        }

        protected override IGuiDrawable GetCurrentDrawable(GuiLabel control)
        {
            throw new System.NotImplementedException();
        }
    }

    public class GuiLabel : GuiControl
    {
        public GuiLabel(GuiLabelStyle style)
        {
            Style = style;
        }

        public GuiLabelStyle Style { get; set; }
        public string Text { get; set; }
    }
}