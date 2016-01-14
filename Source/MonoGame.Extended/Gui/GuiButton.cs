using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Gui
{
    public class GuiButton : GuiDrawableControl
    {
        public GuiButton(GuiStyle normalStyle, GuiStyle pressedStyle, GuiStyle hoveredStyle)
            : base(normalStyle)
        {
            NormalStyle = normalStyle;
            PressedStyle = pressedStyle;
            HoveredStyle = hoveredStyle;
            IsPressed = false;
            IsHovered = false;
        }

        public GuiStyle NormalStyle { get; set; }
        public GuiStyle PressedStyle { get; set; }
        public GuiStyle HoveredStyle { get; set; }

        public bool IsPressed { get; private set; }
        public bool IsHovered { get; private set; }

        private GuiStyle GetCurrentStyle()
        {
            if (IsPressed)
                return PressedStyle;

            if (IsHovered)
                return HoveredStyle;

            return NormalStyle;
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            IsHovered = Shape.Contains(mouseState.X, mouseState.Y);
            IsPressed = IsHovered && mouseState.LeftButton == ButtonState.Pressed;

            SetStyle(GetCurrentStyle());
        }
    }
}