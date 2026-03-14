using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Content;

internal static class TilemapReadHelper
{
    internal static Tilemap ReadMap(ContentReader reader)
    {
        return ReadMapBody(reader, reader.AssetName);
    }

    internal static Tilemap ReadMapWithName(ContentReader reader)
    {
        string name = reader.ReadString();
        return ReadMapBody(reader, name);
    }

    private static Tilemap ReadMapBody(ContentReader reader, string name)
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int tileWidth = reader.ReadInt32();
        int tileHeight = reader.ReadInt32();
        TilemapOrientation orientation = (TilemapOrientation)reader.ReadByte();
        TilemapStaggerAxis staggerAxis = (TilemapStaggerAxis)reader.ReadByte();
        TilemapStaggerIndex staggerIndex = (TilemapStaggerIndex)reader.ReadByte();
        int hexSideLength = reader.ReadInt32();

        bool hasBackground = reader.ReadBoolean();
        Color? backgroundColor = null;

        if (hasBackground)
        {
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            byte a = reader.ReadByte();
            backgroundColor = new Color(r, g, b, a);
        }

        float parallaxOriginX = reader.ReadSingle();
        float parallaxOriginY = reader.ReadSingle();
        int worldX = reader.ReadInt32();
        int worldY = reader.ReadInt32();
        int worldDepth = reader.ReadInt32();

        Tilemap tilemap = new Tilemap(name, width, height, tileWidth, tileHeight, orientation);
        tilemap.StaggerAxis = staggerAxis;
        tilemap.StaggerIndex = staggerIndex;
        tilemap.HexSideLength = hexSideLength;
        tilemap.BackgroundColor = backgroundColor;
        tilemap.ParallaxOrigin = new Vector2(parallaxOriginX, parallaxOriginY);
        tilemap.WorldPosition = new Vector2(worldX, worldY);
        tilemap.WorldDepth = worldDepth;

        TilemapTilesetReader.ReadProperties(reader, tilemap.Properties);
        ReadTilesets(reader, tilemap);
        ReadLayers(reader, tilemap.Layers, tilemap, string.Empty);

        return tilemap;
    }

    private static void ReadTilesets(ContentReader reader, Tilemap tilemap)
    {
        int count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            int firstGlobalId = reader.ReadInt32();
            bool isExternal = reader.ReadBoolean();

            TilemapTileset tileset;

            if (isExternal)
            {
                tileset = reader.ReadExternalReference<TilemapTileset>();
            }
            else
            {
                tileset = TilemapTilesetReader.ReadTileset(reader);
            }

            tileset.FirstGlobalId = firstGlobalId;
            tilemap.Tilesets.Add(tileset);
        }
    }

    private static void ReadLayers(ContentReader reader, TilemapLayerCollection layers, Tilemap tilemap, string pathPrefix)
    {
        int count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            ReadLayerInto(reader, layers, tilemap, pathPrefix);
        }
    }

    private static void ReadLayerInto(ContentReader reader, TilemapLayerCollection layers, Tilemap tilemap, string pathPrefix)
    {
        byte layerType = reader.ReadByte();
        string name = reader.ReadString();
        string cls = reader.ReadString();
        bool isVisible = reader.ReadBoolean();
        float opacity = reader.ReadSingle();

        bool hasTintColor = reader.ReadBoolean();
        Color? tintColor = null;

        if (hasTintColor)
        {
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            byte a = reader.ReadByte();
            tintColor = new Color(r, g, b, a);
        }

        float offsetX = reader.ReadSingle();
        float offsetY = reader.ReadSingle();
        float parallaxX = reader.ReadSingle();
        float parallaxY = reader.ReadSingle();

        TilemapProperties tempProperties = new TilemapProperties();
        TilemapTilesetReader.ReadProperties(reader, tempProperties);

        // Group layers are flattened: recurse with an updated path prefix, adding
        // only leaf layers to the flat collection to match TilemapFactory behavior.
        if (layerType == 3)
        {
            string groupPath = string.IsNullOrEmpty(pathPrefix)
                ? name
                : pathPrefix + "/" + name;

            ReadLayers(reader, layers, tilemap, groupPath);
            return;
        }

        string fullName = string.IsNullOrEmpty(pathPrefix) ? name : pathPrefix + "/" + name;
        TilemapLayer layer = ReadLayerTypeData(reader, layerType, fullName, tilemap);
        layer.Class = cls;
        layer.IsVisible = isVisible;
        layer.Opacity = opacity;
        layer.TintColor = tintColor;
        layer.Offset = new Vector2(offsetX, offsetY);
        layer.ParallaxFactor = new Vector2(parallaxX, parallaxY);

        foreach (KeyValuePair<string, TilemapPropertyValue> property in tempProperties)
        {
            layer.Properties[property.Key] = property.Value;
        }

        layers.Add(layer);
    }

    private static TilemapLayer ReadLayerTypeData(ContentReader reader, byte layerType, string name, Tilemap tilemap)
    {
        switch (layerType)
        {
            case 0:
                return ReadTileLayer(reader, name, tilemap);
            case 1:
                return ReadObjectLayer(reader, name);
            case 2:
                return ReadImageLayer(reader, name);
            case 4:
                return ReadDataLayer(reader, name, tilemap);
            default:
                throw new InvalidOperationException(
                    $"Unknown layer type byte '{layerType}'. " +
                    "The content file may have been built with a different version of MonoGame.Extended.");
        }
    }

    private static TilemapTileLayer ReadTileLayer(ContentReader reader, string name, Tilemap tilemap)
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();

        TilemapTileLayer layer = new TilemapTileLayer(name, width, height, tilemap.TileWidth, tilemap.TileHeight);

        int tileCount = reader.ReadInt32();

        for (int i = 0; i < tileCount; i++)
        {
            ushort x = reader.ReadUInt16();
            ushort y = reader.ReadUInt16();
            int globalId = reader.ReadInt32();
            TilemapTileFlipFlags flags = (TilemapTileFlipFlags)reader.ReadByte();
            layer.SetTile(x, y, new TilemapTile(globalId, flags));
        }

        return layer;
    }

    private static TilemapObjectLayer ReadObjectLayer(ContentReader reader, string name)
    {
        TilemapObjectLayer layer = new TilemapObjectLayer(name);
        layer.DrawOrder = (TilemapObjectDrawOrder)reader.ReadByte();

        int count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            TilemapObject obj = TilemapObjectReader.ReadObject(reader);
            layer.AddObject(obj);
        }

        return layer;
    }

    private static TilemapImageLayer ReadImageLayer(ContentReader reader, string name)
    {
        Texture2D texture = reader.ReadExternalReference<Texture2D>();
        bool repeatX = reader.ReadBoolean();
        bool repeatY = reader.ReadBoolean();

        TilemapImageLayer layer = new TilemapImageLayer(name, texture, Vector2.Zero);
        layer.RepeatX = repeatX;
        layer.RepeatY = repeatY;

        return layer;
    }

    private static TilemapDataLayer ReadDataLayer(ContentReader reader, string name, Tilemap tilemap)
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        return new TilemapDataLayer(name, width, height, tilemap.TileWidth, tilemap.TileHeight);
    }
}
