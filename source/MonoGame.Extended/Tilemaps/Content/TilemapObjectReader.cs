using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tilemaps.Content
{
    /// <summary>
    /// Reads serialized tilemap objects from binary content. Used by both
    /// <see cref="TilemapReader"/> (for object layers) and <see cref="TilemapTilesetReader"/>
    /// (for tile collision objects).
    /// </summary>
    internal static class TilemapObjectReader
    {
        internal static TilemapObject ReadObject(ContentReader reader)
        {
            byte objectType = reader.ReadByte();
            int id = reader.ReadInt32();
            string name = reader.ReadString();
            string cls = reader.ReadString();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float rotation = reader.ReadSingle();
            bool isVisible = reader.ReadBoolean();

            Vector2 position = new Vector2(x, y);
            TilemapObject obj = CreateObject(objectType, id, position);

            obj.Name = name;
            obj.Class = cls;
            obj.Rotation = rotation;
            obj.IsVisible = isVisible;

            TilemapTilesetReader.ReadProperties(reader, obj.Properties);

            ReadObjectTypeData(reader, objectType, obj);

            return obj;
        }

        private static TilemapObject CreateObject(byte objectType, int id, Vector2 position)
        {
            // We need a shell instance before reading type-specific data.
            // Type-specific data is read in ReadObjectTypeData after the common fields.
            switch (objectType)
            {
                case 0: return new TilemapRectangleObject(id, position, Vector2.Zero);
                case 1: return new TilemapEllipseObject(id, position, Vector2.Zero);
                case 2: return new TilemapPointObject(id, position);
                case 3: return new TilemapPolygonObject(id, position, Array.Empty<Vector2>());
                case 4: return new TilemapPolylineObject(id, position, Array.Empty<Vector2>());
                case 5: return new TilemapTileObject(id, position, default, Vector2.Zero);
                case 6: return new TilemapTextObject(id, position, Vector2.Zero, string.Empty);
                default:
                    throw new InvalidOperationException(
                        $"Unknown tilemap object type byte '{objectType}'. " +
                        "The content file may have been built with a different version of MonoGame.Extended.");
            }
        }

        private static void ReadObjectTypeData(ContentReader reader, byte objectType, TilemapObject obj)
        {
            switch (objectType)
            {
                case 0: // Rectangle
                    ((TilemapRectangleObject)obj).Size = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    break;
                case 1: // Ellipse
                    ((TilemapEllipseObject)obj).Size = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    break;
                case 2: // Point
                    break;
                case 3: // Polygon
                    ((TilemapPolygonObject)obj).Points = ReadPoints(reader);
                    break;
                case 4: // Polyline
                    ((TilemapPolylineObject)obj).Points = ReadPoints(reader);
                    break;
                case 5: // Tile
                    ReadTileObjectData(reader, (TilemapTileObject)obj);
                    break;
                case 6: // Text
                    ReadTextObjectData(reader, (TilemapTextObject)obj);
                    break;
            }
        }

        private static Vector2[] ReadPoints(ContentReader reader)
        {
            int count = reader.ReadInt32();
            Vector2[] points = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                points[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }

            return points;
        }

        private static void ReadTileObjectData(ContentReader reader, TilemapTileObject obj)
        {
            int globalId = reader.ReadInt32();
            TilemapTileFlipFlags flipFlags = (TilemapTileFlipFlags)reader.ReadByte();
            float width = reader.ReadSingle();
            float height = reader.ReadSingle();
            obj.Tile = new TilemapTile(globalId, flipFlags);
            obj.Size = new Vector2(width, height);
        }

        private static void ReadTextObjectData(ContentReader reader, TilemapTextObject obj)
        {
            float width = reader.ReadSingle();
            float height = reader.ReadSingle();
            obj.Size = new Vector2(width, height);
            obj.Text = reader.ReadString();
            obj.FontFamily = reader.ReadString();
            obj.PixelSize = reader.ReadInt32();
            obj.WordWrap = reader.ReadBoolean();

            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            byte a = reader.ReadByte();
            obj.Color = new Color(r, g, b, a);

            obj.Bold = reader.ReadBoolean();
            obj.Italic = reader.ReadBoolean();
            obj.Underline = reader.ReadBoolean();
            obj.Strikethrough = reader.ReadBoolean();

            obj.HorizontalAlign = (TilemapTextObjectHorizontalAlignment)reader.ReadByte();
            obj.VerticalAlign = (TilemapTextObjectVerticalAlignment)reader.ReadByte();
        }
    }
}
