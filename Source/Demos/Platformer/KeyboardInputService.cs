using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    public class KeyboardInputService
    {
        public KeyboardInputService()
        {
        }

        public KeyboardState CurrentState { get; private set; }
        public KeyboardState PreviousState { get; private set; }

        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        public bool IsKeyTapped(Keys key)
        {
            return PreviousState.IsKeyDown(key) && CurrentState.IsKeyUp(key);
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }
    }
}