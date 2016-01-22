using MonoGame.Extended.Gui.Drawables;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButtonStyle : GuiControlStyle
    {
        public GuiButtonStyle(IGuiDrawable normal)
            : this(normal, normal, normal)
        {
        }

        public GuiButtonStyle(IGuiDrawable normal, IGuiDrawable pressed)
            : this(normal, pressed, normal)
        {
        }

        public GuiButtonStyle(IGuiDrawable normal, IGuiDrawable pressed, IGuiDrawable hovered)
        {
            Normal = normal;
            Pressed = pressed;
            Hovered = hovered;
        }

        public IGuiDrawable Normal { get; set; }
        public IGuiDrawable Pressed { get; set; }
        public IGuiDrawable Hovered { get; set; }
    }
}