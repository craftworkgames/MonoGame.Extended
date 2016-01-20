using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Styles;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;

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

    public class GuiButton : GuiControl
    {
        private readonly GuiButtonStyle _style;

        public GuiButton(GuiButtonStyle style)
        {
            _style = style;
            IsPressed = false;
        }

        public bool IsPressed { get; private set; }

        public event EventHandler<MouseEventArgs> Clicked;

        protected override IGuiDrawable GetCurrentDrawable()
        {
            if (IsPressed)
                return _style.Pressed;

            if (IsHovered)
                return _style.Hovered;

            return _style.Normal;
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