using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Provides additional methods for working with color
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to its hexadecimal string representation in RGBA format.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>A hexadecimal string representation of the color in the format #RRGGBBAA.</returns>
        public static string ToHex(this Color color)
        {
            string rx = $"{color.R:x2}";
            string gx = $"{color.G:x2}";
            string bx = $"{color.B:x2}";
            string ax = $"{color.A:x2}";

            return $"#{rx}{gx}{bx}{ax}";
        }
    }
}
