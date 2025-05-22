using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ColorHelper
    {
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

        public static Color FromHex(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Color.Transparent;

            if (value[0] == '#')
                value = value.Substring(1);

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

        [Obsolete("Use ColorExtensions.ToHex instead.  This will be removed in the next major SemVer release.")]
        public static string ToHex(Color color)
        {
            var rx = $"{color.R:x2}";
            var gx = $"{color.G:x2}";
            var bx = $"{color.B:x2}";
            var ax = $"{color.A:x2}";
            return $"#{rx}{gx}{bx}{ax}";
        }

        private static readonly Dictionary<string, Color> _colorsByName = typeof(Color)
            .GetRuntimeProperties()
            .Where(p => p.PropertyType == typeof(Color))
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(null), StringComparer.OrdinalIgnoreCase);

        public static Color FromName(string name)
        {
            Color color;

            if (_colorsByName.TryGetValue(name, out color))
                return color;

            throw new InvalidOperationException($"{name} is not a valid color");
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> value based on a packed value in the ABGR format.
        /// </summary>
        /// <remarks>
        /// This is useful for when you have HTML hex style values such as #123456 and want to use it in hex format for
        /// the parameter.  Since Color's standard format is RGBA, you would have to do new Color(0xFF563212) since R
        /// is the LSB.  With this method, you can write it the same way it is written in HTML hex by doing
        /// <c>>ColorExtensions.FromAbgr(0x123456FF);</c>
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
    }
}
