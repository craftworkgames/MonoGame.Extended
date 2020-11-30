using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    public struct KeyboardStateExtended
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public KeyboardStateExtended(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState)
        {
            _currentKeyboardState = currentKeyboardState;
            _previousKeyboardState = previousKeyboardState;
        }

        public bool CapsLock => _currentKeyboardState.CapsLock;
        public bool NumLock => _currentKeyboardState.NumLock;
        public bool IsShiftDown() => _currentKeyboardState.IsKeyDown(Keys.LeftShift) || _currentKeyboardState.IsKeyDown(Keys.RightShift);
        public bool IsControlDown() => _currentKeyboardState.IsKeyDown(Keys.LeftControl) || _currentKeyboardState.IsKeyDown(Keys.RightControl);
        public bool IsAltDown() => _currentKeyboardState.IsKeyDown(Keys.LeftAlt) || _currentKeyboardState.IsKeyDown(Keys.RightAlt);
        public bool IsKeyDown(Keys key) => _currentKeyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => _currentKeyboardState.IsKeyUp(key);
        public Keys[] GetPressedKeys() => _currentKeyboardState.GetPressedKeys();

        public bool WasKeyJustDown(Keys key) => _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
        public bool WasKeyJustUp(Keys key) => _previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);
        public bool WasAnyKeyJustDown() => _previousKeyboardState.GetPressedKeys().Any();
   }
}