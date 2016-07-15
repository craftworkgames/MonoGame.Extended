using MonoGame.Extended.Gui.Wip;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public GuiButton(GuiTemplate style)
            : base(style)
        {
        }

        public bool IsPressed { get; private set; }

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            if(IsEnabled)
                IsPressed = true;

            base.OnMouseDown(sender, args);
        }

        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            IsPressed = false;

            base.OnMouseUp(sender, args);
        }

        //protected override GuiSpriteStyle GetCurrentStyle()
        //{
        //    //if (!IsEnabled)
        //    //    return _style.Disabled;

        //    //if (IsPressed)
        //    //    return _style.Pressed;
            
        //    //if (IsHovered)
        //    //    return _style.Hovered;

        //    //return _style.Normal;
        //    return null;
        //}
    }
}