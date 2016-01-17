using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui
{
    public class GuiButton : GuiDrawableControl
    {
        public GuiButton(GuiControlStyle normalStyle, GuiControlStyle pressedStyle, GuiControlStyle hoveredStyle)
        {
            NormalStyle = normalStyle;
            PressedStyle = pressedStyle;
            HoveredStyle = hoveredStyle;
            IsPressed = false;
        }

        public GuiControlStyle NormalStyle { get; set; }
        public GuiControlStyle PressedStyle { get; set; }
        public GuiControlStyle HoveredStyle { get; set; }

        public bool IsPressed { get; private set; }

        public event EventHandler<MouseEventArgs> Clicked;

        public override GuiControlStyle CurrentStyle
        {
            get
            {
                if (IsPressed)
                    return PressedStyle;

                if (IsHovered)
                    return HoveredStyle;

                return NormalStyle;
            }
        }

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            IsPressed = true;
            base.OnMouseDown(sender, args);
        }

        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            if(IsPressed)
                Clicked.Raise(this, args);

            IsPressed = false;
            base.OnMouseUp(sender, args);
        }
    }
}