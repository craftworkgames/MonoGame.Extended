// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    /// <summary>
    /// Represents the state of keyboard input
    /// </summary>
    /// <remarks>
    /// This is an extended version of the base <see cref="Microsoft.Xna.Framework.Input.KeyboardState"/> struct
    /// that provides utility for checking the state differences between the previous and current state.
    /// </remarks>
    public struct KeyboardStateExtended
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardStateExtended"/> value.
        /// </summary>
        /// <param name="currentKeyboardState">The state of keyboard input during the current update cycle.</param>
        /// <param name="previousKeyboardState">The state of keyboard input during the previous update cycle.</param>
        public KeyboardStateExtended(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState)
        {
            _currentKeyboardState = currentKeyboardState;
            _previousKeyboardState = previousKeyboardState;
        }

        /// <summary>
        /// Gets a value that indicates whether the caps lock key is down during the current state.
        /// </summary>
        public bool CapsLock
        {
#if FNA
            get { return _currentKeyboardState.IsKeyDown(Keys.CapsLock); }
#else
            get { return _currentKeyboardState.CapsLock; }
#endif
        }
        /// <summary>
        /// Gets a value that indicates whether the num lock key is down during the current state.
        /// </summary>
        public bool NumLock
        {
#if FNA
            get { return _currentKeyboardState.IsKeyDown(Keys.NumLock); }
#else
            get { return _currentKeyboardState.NumLock; }
#endif
        }

        /// <summary>
        /// Returns a value that indicates whether either the left or right shift key is down during the current state.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if either the left or right shift key is down during the current state; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool IsShiftDown() => _currentKeyboardState.IsKeyDown(Keys.LeftShift) || _currentKeyboardState.IsKeyDown(Keys.RightShift);

        /// <summary>
        /// Returns a value that indicates whether either the left or right control key is down during the current
        /// state.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if either the left or right control key is down during the current state; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool IsControlDown() => _currentKeyboardState.IsKeyDown(Keys.LeftControl) || _currentKeyboardState.IsKeyDown(Keys.RightControl);

        /// <summary>
        /// Returns a value that indicates whether either the left or righ talt key is currently pressed down.
        /// </summary>
        /// <returns></returns>
        public bool IsAltDown() => _currentKeyboardState.IsKeyDown(Keys.LeftAlt) || _currentKeyboardState.IsKeyDown(Keys.RightAlt);

        /// <summary>
        /// Returns a value that indicates if the specified key is down during the current state.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <see langword="true"/> if the key is down during the current state; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsKeyDown(Keys key) => _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Returns a value that indicates if the specified key is up during the current state.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <see langword="true"/> if the key is up during the current state; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsKeyUp(Keys key) => _currentKeyboardState.IsKeyUp(key);

        /// <summary>
        /// Returns the total number of keys down during the current state.
        /// </summary>
        public int GetPressedKeyCount => _currentKeyboardState.GetPressedKeyCount();

        /// <summary>
        /// Returns an array of all keys that are down during the current state.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Keys"/> values that represent each key that is down during the current state.
        /// </returns>
        public Keys[] GetPressedKeys() => _currentKeyboardState.GetPressedKeys();

        /// <summary>
        /// Fills an existing array with the keys pressed during the current state.
        /// </summary>
        /// <param name="keys">An existing array to fill with the pressed keys.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="keys"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the array provided by the <paramref name="keys"/> parameter is not large enough to fit all
        /// pressed keys.  Use <see cref="GetPressedKeyCount"/> to determine the total number of elements.
        /// </exception>
        public void GetPressedKeys(Keys[] keys) => _currentKeyboardState.GetPressedKeys(keys);

        /// <summary>
        /// Returns whether the given key was down during previous state, but is now up.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <see langword="true"/> if the key was released this state-change, otherwise <see langword="false"/>.
        /// </returns>
        public readonly bool WasKeyReleased(Keys key) => _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);

        /// <summary>
        /// Returns whether the given key was up during previous state, but is now down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <see langword="true"/> if the key was pressed this state-change, otherwise <see langword="false"/>.
        /// </returns>
        public readonly bool WasKeyPressed(Keys key) => _previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Returns whether any key was pressed down on the previous state.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if any key was pressed during the previous state; otherwise, <see langword="false"/>.
        /// </returns>
        public bool WasAnyKeyJustDown() => _previousKeyboardState.GetPressedKeyCount() > 0;
   }
}
