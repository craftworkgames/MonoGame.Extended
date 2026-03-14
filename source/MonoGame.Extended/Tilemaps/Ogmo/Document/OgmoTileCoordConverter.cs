using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

// Ogmo encodes tile coordinates as [[tileX, tileY], [-1], ...] where a single-element
// array containing a negative value signals an empty cell. These converters translate
// that format into List<Point?> / List<List<Point?>> so callers never see raw int arrays.

internal sealed class OgmoTileCoordListConverter : JsonConverter<List<Point?>>
{
    public override List<Point?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<Point?> result = new List<Point?>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            return result;
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            result.Add(ReadCoordPair(ref reader));
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<Point?> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (Point? point in value)
        {
            writer.WriteStartArray();

            if (point.HasValue)
            {
                writer.WriteNumberValue(point.Value.X);
                writer.WriteNumberValue(point.Value.Y);
            }
            else
            {
                writer.WriteNumberValue(-1);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }

    internal static Point? ReadCoordPair(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            return null;
        }

        int x = -1;
        int y = -1;
        int count = 0;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (count == 0)
            {
                x = reader.GetInt32();
            }
            else if (count == 1)
            {
                y = reader.GetInt32();
            }

            count++;
        }

        // A pair with two non-negative values is a valid [tileX, tileY] coordinate.
        // A single-element array (count == 1) means empty regardless of the value.
        if (count >= 2)
        {
            return new Point(x, y);
        }

        return null;
    }
}

internal sealed class OgmoTileCoordList2DConverter : JsonConverter<List<List<Point?>>>
{
    public override List<List<Point?>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<List<Point?>> result = new List<List<Point?>>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            return result;
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            List<Point?> row = new List<Point?>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    row.Add(OgmoTileCoordListConverter.ReadCoordPair(ref reader));
                }
            }

            result.Add(row);
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<List<Point?>> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (List<Point?> row in value)
        {
            writer.WriteStartArray();

            foreach (Point? point in row)
            {
                writer.WriteStartArray();

                if (point.HasValue)
                {
                    writer.WriteNumberValue(point.Value.X);
                    writer.WriteNumberValue(point.Value.Y);
                }
                else
                {
                    writer.WriteNumberValue(-1);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}
