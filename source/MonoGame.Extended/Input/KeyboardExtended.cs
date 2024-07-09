﻿using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    public static class KeyboardExtended
    {
        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _previousKeyboardState;

        public static KeyboardStateExtended GetState()
        {
            return new KeyboardStateExtended(_currentKeyboardState, _previousKeyboardState);
        }

        public static void Refresh()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
        }
    }
}