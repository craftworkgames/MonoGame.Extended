using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    public static class KeyboardStateExtensions
    {

#if FNA
        // MomoGame compatibility layer

        /// <summary>
        /// Fills an array of values holding keys that are currently being pressed.
        /// Note: This extension method is not allocation free when targeting FNA.
        /// </summary>
        /// <param name="keys">The keys array to fill.
        internal static void GetPressedKeys(this KeyboardState value, Keys[] keys)
        {
            Keys[] pressedKeys = value.GetPressedKeys();
            Array.Copy(pressedKeys, keys, pressedKeys.Length);
        }

        /// <summary>
        /// Returns the number of pressed keys in this KeyboardState.
        /// </summary>
        /// Note: This extension method is not allocation free when targeting FNA.
        /// <returns>An integer representing the number of keys currently pressed in this KeyboardState.</returns>

        internal static int GetPressedKeyCount(this KeyboardState value)
        {
            Keys[] pressedKeys = value.GetPressedKeys();
            return pressedKeys.Length;
        }
#endif
    }
}