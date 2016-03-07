using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapProcessorResult>
    {
        protected override void Write(ContentWriter writer, TiledMapProcessorResult value)
        {
            var map = value.Map;
            writer.Write(HexToColor(map.BackgroundColor));
            writer.Write(ConvertRenderOrder(map.RenderOrder).ToString());
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.TileWidth);
            writer.Write(map.TileHeight);
            writer.Write(Convert.ToInt32(map.Orientation));
            WriteCustomProperties(writer, map.Properties);

            writer.Write(map.Tilesets.Count);

            foreach (var tileset in map.Tilesets)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                writer.Write(PathHelper.RemoveExtension(tileset.Image.Source));
                writer.Write(tileset.FirstGid);
                writer.Write(tileset.TileWidth);
                writer.Write(tileset.TileHeight);
                writer.Write(tileset.Spacing);
                writer.Write(tileset.Margin);
                WriteCustomProperties(writer, tileset.Properties);
            }

            writer.Write(map.Layers.Count);

            foreach (var layer in map.Layers)
            {
                writer.Write(layer.Name);
                writer.Write(layer.Visible);
                writer.Write(layer.Opacity);

                var tileLayer = layer as TmxTileLayer;

                if (tileLayer != null)
                {
                    writer.Write("TileLayer");
                    writer.Write(tileLayer.Data.Tiles.Count);

                    foreach (var tile in tileLayer.Data.Tiles)
                        writer.Write(tile.Gid);

                    writer.Write(tileLayer.Width); 
                    writer.Write(tileLayer.Height);
                }

                var imageLayer = layer as TmxImageLayer;

                if (imageLayer != null)
                {
                    writer.Write("ImageLayer");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    writer.Write(PathHelper.RemoveExtension(imageLayer.Image.Source));
                    writer.Write(new Vector2(imageLayer.X, imageLayer.Y));
                }

                WriteCustomProperties(writer, layer.Properties);
            }

            writer.Write(map.ObjectGroups.Count);

            foreach (var objectGroup in map.ObjectGroups)
            {
                writer.Write(objectGroup.Name);
                writer.Write(objectGroup.Visible);
                writer.Write(objectGroup.Opacity);

                writer.Write(objectGroup.Objects.Count);

                foreach (var tmxObject in objectGroup.Objects)
                {
                    var objectType = GetObjectType(tmxObject);

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

                WriteCustomProperties(writer, objectGroup.Properties);
            }
        }

        private static void WritePolyPoints(ContentWriter writer, string polyPointsString)
        {
            var points = polyPointsString.Split(' ')
                            .Select(p => { var xy = p.Split(','); return new Vector2(float.Parse(xy[0]), float.Parse(xy[1])); })
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