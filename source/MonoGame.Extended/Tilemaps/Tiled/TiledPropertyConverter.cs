using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Tilemaps.Tiled;

internal static class TiledPropertyConverter
{
    public static void Convert(TiledPropertiesXml sourceProperties, TilemapProperties targetProperties)
    {
        if (sourceProperties?.Properties == null)
        {
            return;
        }

        foreach (TiledPropertyXml property in sourceProperties.Properties)
        {
            if (string.IsNullOrEmpty(property.Name))
            {
                continue;
            }

            ConvertProperty(property, targetProperties);
        }
    }

    private static void ConvertProperty(TiledPropertyXml property, TilemapProperties targetProperties)
    {
        string type = property.Type ?? "string";

        try
        {
            switch (type.ToLowerInvariant())
            {
                case "string":
                    targetProperties.SetString(property.Name, property.Value ?? string.Empty);
                    break;

                case "int":
                    if (int.TryParse(property.Value, out int intValue))
                    {
                        targetProperties.SetInt(property.Name, intValue);
                    }
                    else
                    {
                        throw new FormatException($"Invalid int value: '{property.Value}'");
                    }

                    break;

                case "float":
                    if (float.TryParse(property.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                    {
                        targetProperties.SetFloat(property.Name, floatValue);
                    }
                    else
                    {
                        throw new FormatException($"Invalid float value: '{property.Value}'");
                    }

                    break;

                case "bool":
                    if (bool.TryParse(property.Value, out bool boolValue))
                    {
                        targetProperties.SetBool(property.Name, boolValue);
                    }
                    else
                    {
                        throw new FormatException($"Invalid bool value: '{property.Value}'");
                    }

                    break;

                case "color":
                    Color? color = TiledColorParser.Parse(property.Value);

                    if (color.HasValue)
                    {
                        targetProperties.SetColor(property.Name, color.Value);
                    }
                    else
                    {
                        throw new FormatException($"Invalid color value: '{property.Value}'");
                    }

                    break;

                case "file":
                    targetProperties.SetString(property.Name, property.Value ?? string.Empty);
                    break;

                case "object":
                    if (int.TryParse(property.Value, out int objectId))
                    {
                        targetProperties.SetInt(property.Name, objectId);
                    }
                    else
                    {
                        throw new FormatException($"Invalid object ID: '{property.Value}'");
                    }

                    break;

                case "class":
                    targetProperties.SetString(property.Name, property.Value ?? string.Empty);
                    break;

                default:
                    targetProperties.SetString(property.Name, property.Value ?? string.Empty);
                    break;
            }
        }
        catch (Exception ex)
        {
            throw new TilemapParseException(
                $"Failed to convert property '{property.Name}' of type '{type}': {ex.Message}", ex);
        }
    }
}
