using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Maps.Tiled;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapProcessorResult>
    {
        protected override void Write(ContentWriter writer, TiledMapProcessorResult value)
        {
            value.Logger.LogMessage("Writing Tiled map...");

            var map = value.Map;
            writer.Write(HexToColor(map.BackgroundColor));
            writer.Write(ConvertRenderOrder(map.RenderOrder).ToString());
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.TileWidth);
            writer.Write(map.TileHeight);
            writer.Write(Convert.ToInt32(map.Orientation));
            WriteCustomProperties(writer, map.Properties);

            value.Logger.LogMessage("Writing tilesets...");
            writer.Write(map.Tilesets.Count);

            foreach (var tileset in map.Tilesets)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                writer.Write(Path.ChangeExtension(tileset.Image.Source, null));
                writer.Write(HexToColor(tileset.Image.Trans));
                writer.Write(tileset.FirstGid);
                writer.Write(tileset.TileWidth);
                writer.Write(tileset.TileHeight);
                writer.Write(tileset.TileCount);
                writer.Write(tileset.Spacing);
                writer.Write(tileset.Margin);

                writer.Write(tileset.Tiles.Count);

                foreach(var tile in tileset.Tiles)
                {
                    writer.Write(tile.Id);
                    writer.Write(tile.Frames.Count);

                    foreach(var frame in tile.Frames)
                    {
                        writer.Write(frame.TileId);
                        writer.Write(frame.Duration);
                    }

                    WriteCustomProperties(writer, tile.Properties);
                }

                WriteCustomProperties(writer, tileset.Properties);
            }

            value.Logger.LogMessage("Writing layers...");
            writer.Write(map.Layers.Count);

            foreach (var layer in map.Layers)
            {
                value.Logger.LogMessage($"Writing {layer.GetType().Name} layer: {layer.Name}");

                writer.Write(layer.Name);
                writer.Write(layer.Visible);
                writer.Write(layer.Opacity);
                writer.Write(layer.OffsetX);
                writer.Write(layer.OffsetY);

                var tileLayer = layer as TmxTileLayer;

                if (tileLayer != null)
                {
                    writer.Write("TileLayer");
                    writer.Write(tileLayer.Data.Tiles.Count);

                    foreach (var tile in tileLayer.Data.Tiles)
                        writer.Write(tile.Gid);

                    writer.Write(tileLayer.Width);
                    writer.Write(tileLayer.Height);
                    WriteCustomProperties(writer, layer.Properties);
                }

                var imageLayer = layer as TmxImageLayer;

                if (imageLayer != null)
                {
                    writer.Write("ImageLayer");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    writer.Write(Path.ChangeExtension(imageLayer.Image.Source, null));
                    writer.Write(new Vector2(imageLayer.X, imageLayer.Y));
                    WriteCustomProperties(writer, layer.Properties);
                }

                var objectLayer = layer as TmxObjectLayer;

                if (objectLayer != null)
                {
                    writer.Write("ObjectLayer");
                    WriteObjectLayer(writer, objectLayer, value.Logger);
                }
            }
        }

        private static void WritePolyPoints(ContentWriter writer, string polyPointsString)
        {
            var points = polyPointsString.Split(' ')
                            .Select(p =>
                            {
                                var xy = p.Split(',');
                                var x = float.Parse(xy[0], CultureInfo.InvariantCulture.NumberFormat);
                                var y = float.Parse(xy[1], CultureInfo.InvariantCulture.NumberFormat);
                                return new Vector2(x, y);
                            })
                            .ToArray();

            writer.Write(points.Length);

            foreach (var point in points)
                writer.Write(point);
        }

        public TiledObjectType GetObjectType(TmxObject tmxObject)
        {
            if(tmxObject.Gid >= 0)
                return TiledObjectType.Tile;

            if(tmxObject.Ellipse != null)
                return TiledObjectType.Ellipse;

            if(tmxObject.Polygon != null)
                return TiledObjectType.Polygon;

            if(tmxObject.Polyline != null)
                return TiledObjectType.Polyline;
            
            return TiledObjectType.Rectangle;
        }

        private void WriteObjectLayers(ContentWriter writer, IReadOnlyCollection<TmxObjectLayer> groups)
        {
            writer.Write(groups.Count);

            foreach (var objectLayer in groups)
                WriteObjectLayer(writer, objectLayer);
        }

        private void WriteObjectLayer(ContentWriter writer, TmxObjectLayer layer, ContentBuildLogger logger = null)
        {
            writer.Write(layer.Name ?? string.Empty);
            writer.Write(layer.Visible);
            writer.Write(layer.Opacity);

            writer.Write(layer.Objects.Count);

            foreach (var tmxObject in layer.Objects)
            {
                var objectType = GetObjectType(tmxObject);

                logger?.LogMessage(
                    $"Writing {objectType} object: {tmxObject.Name ?? tmxObject.Id.ToString()} [({tmxObject.X}, {tmxObject.Y}) {tmxObject.Width}x{tmxObject.Height}]");

                writer.Write((int)objectType);
                writer.Write(tmxObject.Id);
                writer.Write(tmxObject.Gid);
                writer.Write(tmxObject.X);
                writer.Write(tmxObject.Y);
                writer.Write(tmxObject.Width);
                writer.Write(tmxObject.Height);
                writer.Write(tmxObject.Rotation);

                writer.Write(tmxObject.Name ?? string.Empty);
                writer.Write(tmxObject.Type ?? string.Empty);
                writer.Write(tmxObject.Visible);

                if (objectType == TiledObjectType.Polygon)
                    WritePolyPoints(writer, tmxObject.Polygon.Points);

                if (objectType == TiledObjectType.Polyline)
                    WritePolyPoints(writer, tmxObject.Polyline.Points);

                WriteCustomProperties(writer, tmxObject.Properties);
            }

            WriteCustomProperties(writer, layer.Properties);
        }

        private static void WriteCustomProperties(ContentWriter writer, IReadOnlyCollection<TmxProperty> properties)
        {
            writer.Write(properties.Count);

            foreach (var mapProperty in properties)
            {
                writer.Write(mapProperty.Name);
                writer.Write(mapProperty.Value);
            }
        }

        private static Color HexToColor(string hexValue)
        {
            if (string.IsNullOrEmpty(hexValue))
                return new Color(128, 128, 128);

            hexValue = hexValue.TrimStart('#');
            var r = int.Parse(hexValue.Substring(0, 2), NumberStyles.HexNumber);
            var g = int.Parse(hexValue.Substring(2, 2), NumberStyles.HexNumber);
            var b = int.Parse(hexValue.Substring(4, 2), NumberStyles.HexNumber);
            return new Color(r, g, b);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TiledMap).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TiledMapReader).AssemblyQualifiedName;
        }

        private static TiledRenderOrder ConvertRenderOrder(TmxRenderOrder renderOrder)
        {
            switch (renderOrder)
            {
                case TmxRenderOrder.RightDown:
                    return TiledRenderOrder.RightDown;
                case TmxRenderOrder.RightUp:
                    return TiledRenderOrder.RightUp;
                case TmxRenderOrder.LeftDown:
                    return TiledRenderOrder.LeftDown;
                case TmxRenderOrder.LeftUp:
                    return TiledRenderOrder.LeftUp;
                default:
                    return TiledRenderOrder.RightDown;
            }
        }
    }
}
