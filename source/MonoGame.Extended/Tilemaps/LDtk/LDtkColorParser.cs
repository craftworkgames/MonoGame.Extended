using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps.LDtk
{
    internal static class LDtkColorParser
    {
        public static Color ParseColor(string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return Color.Transparent;
            }

            if (!colorString.StartsWith('#'))
            {
                return Color.Transparent;
            }

            string hex = colorString.Substring(1);

            if (hex.Length != 6)
            {
                return Color.Transparent;
            }

            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);

                return new Color(r, g, b, 255);
            }
            catch (FormatException)
            {
                return Color.Transparent;
            }
        }

        public static bool TryParseColor(string colorString, out Color color)
        {
            color = ParseColor(colorString);
            return color != Color.Transparent || colorString == "#000000";
        }
    }
}
