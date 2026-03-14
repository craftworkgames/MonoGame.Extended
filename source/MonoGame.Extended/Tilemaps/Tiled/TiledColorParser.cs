using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tilemaps.Tiled;

internal static class TiledColorParser
{
    public static Color? Parse(string colorString)
    {
        if (string.IsNullOrWhiteSpace(colorString))
        {
            return null;
        }

        if (!colorString.StartsWith("#"))
        {
            throw new TilemapParseException(
                $"Invalid color format: '{colorString}'. Expected format: #RRGGBB or #AARRGGBB");
        }

        string hex = colorString.Substring(1);

        try
        {
            if (hex.Length == 6)
            {
                int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                return new Color(r, g, b, 255);
            }
            else if (hex.Length == 8)
            {
                int a = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                int r = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                return new Color(r, g, b, a);
            }
            else
            {
                throw new TilemapParseException(
                    $"Invalid color format: '{colorString}'. Expected 6 or 8 hex digits after #");
            }
        }
        catch (FormatException ex)
        {
            throw new TilemapParseException(
                $"Invalid color format: '{colorString}'. Failed to parse hex digits", ex);
        }
    }

    public static bool TryParse(string colorString, out Color? color)
    {
        try
        {
            color = Parse(colorString);
            return true;
        }
        catch (Exception)
        {
            color = null;
            return false;
        }
    }
}
