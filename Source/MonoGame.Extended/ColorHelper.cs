using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ColorHelper
    {
        public static Color ToColor(this string hex)
        {
            if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) || hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
            {
                hex = hex.Substring(2);
            }
            else if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            if (hex.Length < 6)
            {
                hex = hex.PadLeft(6, '0');
            }

            if (hex.Length < 8)
            {
                hex = hex.PadLeft(8, 'F');
            }

            var argb = uint.Parse(hex, NumberStyles.HexNumber);
            return argb.ToColor();
        }

        public static Color ToColor(this uint argb)
        {
            var alpha = (byte)((argb & 0xFF000000) >> 0x18);
            var red = (byte)((argb & 0xFF0000) >> 0x10);
            var green = (byte)((argb & 0xFF00) >> 0x8);
            var blue = (byte)(argb & 0xFF);
            return Color.FromNonPremultiplied(red, green, blue, alpha);
        }
    }
}
