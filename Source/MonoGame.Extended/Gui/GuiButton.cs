using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
            IsHovered = false;
        }

        public GuiControlStyle NormalStyle { get; set; }
        public GuiControlStyle PressedStyle { get; set; }
        public GuiControlStyle HoveredStyle { get; set; }

        public bool IsPressed { get; private set; }
        public bool IsHovered { get; private set; }

        public event EventHandler Clicked;

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

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var previouslyPressed = IsPressed;

            IsHovered = Shape.Contains(mouseState.X, mouseState.Y);
            IsPressed = IsHovered && mouseState.LeftButton == ButtonState.Pressed;

            if(previouslyPressed && !IsPressed && IsHovered)
                Clicked.Raise(this, EventArgs.Empty);
        }
    }
}