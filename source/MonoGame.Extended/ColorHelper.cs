using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ColorHelper
    {
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
                var q = hsl.Z < 0.5f ? hsl.Z*(1.0f + hsl.Y) : hsl.Z + hsl.Y - hsl.Z*hsl.Y;
                var p = 2.0f*hsl.Z - q;

                color.X = HueToRgb(p, q, hsl.X + 1.0f/3.0f);
                color.Y = HueToRgb(p, q, hsl.X);
                color.Z = HueToRgb(p, q, hsl.X - 1.0f/3.0f);
            }

            return new Color(color);
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0.0f) t += 1.0f;
            if (t > 1.0f) t -= 1.0f;
            if (t < 1.0f/6.0f) return p + (q - p)*6.0f*t;
            if (t < 1.0f/2.0f) return q;
            if (t < 2.0f/3.0f) return p + (q - p)*(2.0f/3.0f - t)*6.0f;
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
            .ToDictionary(p => p.Name, p => (Color) p.GetValue(null), StringComparer.OrdinalIgnoreCase);

        public static Color FromName(string name)
        {
            Color color;

            if(_colorsByName.TryGetValue(name, out color))
                return color;

            throw new InvalidOperationException($"{name} is not a valid color");
        }
    }
}
