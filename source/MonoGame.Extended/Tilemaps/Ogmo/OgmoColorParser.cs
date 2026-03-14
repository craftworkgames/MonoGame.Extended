using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps.Ogmo;

internal static class OgmoColorParser
{
    public static Color ParseColor(string colorString)
    {
        if (string.IsNullOrEmpty(colorString))
        {
            return Color.White;
        }

        if (!colorString.StartsWith("#"))
        {
            return Color.White;
        }

        string hex = colorString.Substring(1);

        if (hex.Length != 8)
        {
            return Color.White;
        }

        try
        {
            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            int a = Convert.ToInt32(hex.Substring(6, 2), 16);

            return new Color(r, g, b, a);
        }
        catch (FormatException)
        {
            return Color.White;
        }
    }
}
