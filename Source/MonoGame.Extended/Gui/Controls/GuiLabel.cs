using MonoGame.Extended.Gui.Styles;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabel : GuiControl
    {
        public GuiLabel(GuiTextStyle textStyle)
        {
            TextStyle = textStyle;
        }

        public GuiTextStyle TextStyle { get; set; }
        public string Text { get; set; }

        public override GuiControlStyle CurrentStyle
        {
            get { return TextStyle; }
        }
    }
}