using System;
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
        public void GetPressedKeys(Keys[] keys) => _currentKeyboardState.GetPressedKeys(keys);
      
        /// <summary>
        /// Gets whether the given key was down on the previous state, but is now up.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was released this state-change, otherwise false.</returns>
        [Obsolete($"Deprecated in favor of {nameof(IsKeyReleased)}")]
        public bool WasKeyJustDown(Keys key) => _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);

        /// <summary>
        /// Gets whether the given key was up on the previous state, but is now down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was pressed this state-change, otherwise false.</returns>
        [Obsolete($"Deprecated in favor of {nameof(IsKeyPressed)}")]
        public bool WasKeyJustUp(Keys key) => _previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Gets whether the given key was down on the previous state, but is now up.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was released this state-change, otherwise false.</returns>
        public readonly bool IsKeyReleased(Keys key) => _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);

        /// <summary>
        /// Gets whether the given key was up on the previous state, but is now down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was pressed this state-change, otherwise false.</returns>
        public readonly bool IsKeyPressed(Keys key) => _previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);

        public bool WasAnyKeyJustDown() => _previousKeyboardState.GetPressedKeyCount() > 0;
   }
}
