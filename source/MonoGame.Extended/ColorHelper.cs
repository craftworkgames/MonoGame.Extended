using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Provides utility methods for working with <see cref="Color"/> values.
    /// </summary>
    public static class ColorHelper
    {
        private static readonly Dictionary<string, Color> s_colorsByName = typeof(Color)
            .GetRuntimeProperties()
            .Where(p => p.PropertyType == typeof(Color))
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(null), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Converts a hexadecimal color string to a <see cref="Color"/> value.
        /// </summary>
        /// <param name="value">
        /// The hexadecimal color string to convert. Supports multiple formats:
        /// <list type="bullet">
        ///     <item>3 characters: RGB shorthand (e.g., "F0A" becomes "FF00AA")</item>
        ///     <item>4 characters: RGBA shorthand (e.g., "F0A8" becomes "FF00AA88")</item>
        ///     <item>6 characters: Full RGB format (e.g., "FF00AA")</item>
        ///     <item>8 characters: Full RGBA format (e.g., "FF00AA88")</item>
        /// </list>
        /// Optional '#' prefix is automatically handled and removed.
        /// </param>
        /// <returns>
        /// A <see cref="Color"/> value representing the parsed hexadecimal color, or <see cref="Color.Transparent"/>
        /// if the input is <see langword="null"/> or an empty string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The length of <paramref name="value"/> is not 3, 4, 6, or 8 (excluding a '#' prefix)
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="value"/> contains invalid hexadecimal characters.
        /// </exception>
        /// <exception cref="OverflowException">
        /// <paramref name="value"/> represents a value too large for a 32-bit unsigned integer.
        /// </exception>
        public static Color FromHex(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.Transparent;
            }

            return FromHex(value.AsSpan());
        }

        /// <summary>
        /// Converts a hexadecimal color span to a <see cref="Color"/> value.
        /// </summary>
        /// <remarks>
        /// This overload provides better performance than the string version by avoiding string allocations
        /// when removing the '#' prefix and during parsing operations. Particularly beneficial when 
        /// processing large numbers of hex colors or when called frequently.
        /// </remarks>
        /// <param name="value">
        /// A read-only span of characters representing the hexadecimal color. Supports multiple formats:
        /// <list type="bullet">
        ///     <item>3 characters: RGB shorthand (e.g., "F0A" becomes "FF00AA")</item>
        ///     <item>4 characters: RGBA shorthand (e.g., "F0A8" becomes "FF00AA88")</item>
        ///     <item>6 characters: Full RGB format (e.g., "FF00AA")</item>
        ///     <item>8 characters: Full RGBA format (e.g., "FF00AA88")</item>
        /// </list>
        /// Optional '#' prefix is automatically handled and removed.
        /// </param>
        /// <returns>
        /// A <see cref="Color"/> value representing the parsed hexadecimal color, or <see cref="Color.Transparent"/>
        /// if the input is is empty.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The length of <paramref name="value"/> is not 3, 4, 6, or 8 (excluding a '#' prefix)
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="value"/> contains invalid hexadecimal characters.
        /// </exception>
        /// <exception cref="OverflowException">
        /// <paramref name="value"/> represents a value too large for a 32-bit unsigned integer.
        /// </exception>
        public static Color FromHex(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return Color.Transparent;
            }

            if (value[0] == '#')
            {
                value = value.Slice(1);
            }

            int r, g, b, a;
            uint hexInt = uint.Parse(value, System.Globalization.NumberStyles.HexNumber);
            switch (value.Length)
            {
                case 6:
                    r = (byte)((hexInt & 0x00FF0000) >> 16);
                    g = (byte)((hexInt & 0x0000FF00) >> 8);
                    b = (byte)(hexInt & 0x000000FF);
                    a = 255;
                    break;

                case 8:
                    r = (byte)((hexInt & 0xFF000000) >> 24);
                    g = (byte)((hexInt & 0x00FF0000) >> 16);
                    b = (byte)((hexInt & 0x0000FF00) >> 8);
                    a = (byte)(hexInt & 0x000000FF);
                    break;

                case 3:
                    r = (byte)(((hexInt & 0x00000F00) | (hexInt & 0x00000F00) << 4) >> 8);
                    g = (byte)(((hexInt & 0x000000F0) | (hexInt & 0x000000F0) << 4) >> 4);
                    b = (byte)((hexInt & 0x0000000F) | (hexInt & 0x0000000F) << 4);
                    a = 255;
                    break;

                case 4:
                    r = (byte)(((hexInt & 0x0000F000) | (hexInt & 0x0000F000) << 4) >> 12);
                    g = (byte)(((hexInt & 0x00000F00) | (hexInt & 0x00000F00) << 4) >> 8);
                    b = (byte)(((hexInt & 0x000000F0) | (hexInt & 0x000000F0) << 4) >> 4);
                    a = (byte)((hexInt & 0x0000000F) | (hexInt & 0x0000000F) << 4);
                    break;

                default:
                    throw new ArgumentException($"Malformed hexadecimal color: {value}");
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> value from the specified name of a predefined color.
        /// Gets a <see cref="Color"/> value from a p
        /// </summary>
        /// <param name="name">The name of the predefined color.</param>
        /// <returns>
        /// The <see cref="Color"/> value this method creates.
        /// </returns>
        /// <exception cref="InvalidOperationException"><paramref name="name"/> is not a valid color.</exception>
        public static Color FromName(string name)
        {
            if (s_colorsByName.TryGetValue(name, out Color color))
            {
                return color;
            }

            throw new InvalidOperationException($"{name} is not a valid color");
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> value based on a packed value in the ABGR format.
        /// </summary>
        /// <remarks>
        /// This is useful for when you have HTML hex style values such as #123456 and want to use it in hex format for
        /// the parameter.  Since Color's standard format is RGBA, you would have to do new Color(0xFF563212) since R
        /// is the LSB.  With this method, you can write it the same way it is written in HTML hex by doing
        /// <c>ColorHelper.FromAbgr(0x123456FF);</c>
        /// </remarks>
        /// <param name="abgr">The packed color value in ABGR format</param>
        /// <returns>The <see cref="Color"/> value created</returns>
        public static Color FromAbgr(uint abgr)
        {
            uint rgba = (abgr & 0x000000FF) << 24 | // Alpha
                        (abgr & 0x0000FF00) << 8 | // Blue
                        (abgr & 0x00FF0000) >> 8 | // Green
                        (abgr & 0xFF000000) >> 24;  // Red

            Color result;

#if FNA
            result = default;
            result.PackedValue = rgba;
#else
            result = new Color(rgba);
#endif

            return result;
        }

        [Obsolete("Use HslColor.ToRgb instead.  This will be removed in the next major SemVer release.")]
        //http://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
        public static Color FromHsl(float hue, float saturation, float lightness)
        {
            var hsl = new Vector4(hue, saturation, lightness, 1);
            var color = new Vector4(0, 0, 0, hsl.W);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (hsl.Y == 0.0f)
                color.X = color.Y = color.Z = hsl.Z;
            else
            {
                var q = hsl.Z < 0.5f ? hsl.Z * (1.0f + hsl.Y) : hsl.Z + hsl.Y - hsl.Z * hsl.Y;
                var p = 2.0f * hsl.Z - q;

                color.X = HueToRgb(p, q, hsl.X + 1.0f / 3.0f);
                color.Y = HueToRgb(p, q, hsl.X);
                color.Z = HueToRgb(p, q, hsl.X - 1.0f / 3.0f);
            }

            return new Color(color);
        }

        [Obsolete("This will be removed in the next major SemVer release")]
        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0.0f) t += 1.0f;
            if (t > 1.0f) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        [Obsolete("Use ColorExtensions.ToHex instead.  This will be removed in the next major SemVer release.")]
        public static string ToHex(Color color)
        {
            var rx = $"{color.R:x2}";
            var gx = $"{color.G:x2}";
            var bx = $"{color.B:x2}";
            var ax = $"{color.A:x2}";
            return $"#{rx}{gx}{bx}{ax}";
        }

    }
}
