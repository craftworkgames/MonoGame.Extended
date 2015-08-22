using System;

namespace Microsoft.Xna.Framework.Input
{
    public class KeyboardKeyEventArgs : EventArgs
    {
        public Keys Key { get; private set; }
        public Modifiers Modifiers { get; private set; }

        protected internal KeyboardKeyEventArgs(Keys key) 
        {
            Key = key;

            var current = Keyboard.GetState();
            Modifiers = Modifiers.None;
            if (current.IsKeyDown(Keys.LeftControl) || current.IsKeyDown(Keys.RightControl))
            {
                Modifiers |= Modifiers.Control;
            }
            if (current.IsKeyDown(Keys.LeftShift) || current.IsKeyDown(Keys.RightShift))
            {
                Modifiers |= Modifiers.Shift;
            }
            if (current.IsKeyDown(Keys.LeftAlt) || current.IsKeyDown(Keys.RightAlt))
            {
                Modifiers |= Modifiers.Alt;
            }
        }
    }
}
