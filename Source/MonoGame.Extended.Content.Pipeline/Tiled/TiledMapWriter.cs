using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapContent>
    {
        protected override void Write(ContentWriter writer, TiledMapContent map)
        {
            WriteMetaData(writer, map);
            WriteTilesets(writer, map.Tilesets);
            WriteLayers(writer, map.Layers);
        }

        private static void WriteMetaData(ContentWriter writer, TiledMapContent map)
        {
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.TileWidth);
            writer.Write(map.TileHeight);
            writer.Write(ColorHelper.FromHex(map.BackgroundColor));
            writer.Write((byte)map.RenderOrder);
            writer.Write((byte)map.Orientation);
            writer.WriteTiledMapProperties(map.Properties);
        }

        private static void WriteTilesets(ContentWriter output, IReadOnlyCollection<TiledMapTilesetContent> tilesets)
        {
            output.Write(tilesets.Count);
            foreach (var tileset in tilesets)
                WriteTileset(output, tileset);
        }

        private static void WriteTileset(ContentWriter output, TiledMapTilesetContent tileset)
        {
            output.Write(Path.ChangeExtension(tileset.Image.Source, null));
            output.Write(tileset.FirstGlobalIdentifier);
            output.Write(tileset.TileWidth);
            output.Write(tileset.TileHeight);
            output.Write(tileset.TileCount);
            output.Write(tileset.Spacing);
            output.Write(tileset.Margin);

            output.Write(tileset.Columns);

            output.Write(tileset.Tiles.Count);
            foreach (var tilesetTile in tileset.Tiles)
                WriteTilesetTile(output, tilesetTile);

            output.WriteTiledMapProperties(tileset.Properties);
        }

        private static void WriteTilesetTile(ContentWriter output, TiledMapTilesetTileContent tilesetTile)
        {
            output.Write(tilesetTile.LocalIdentifier);

            output.Write(tilesetTile.Frames.Count);

            output.Write(tilesetTile.Objects.Count);
            foreach (var @object in tilesetTile.Objects)
                WriteObject(output, @object);

            foreach (var frame in tilesetTile.Frames)
            {
                output.Write(frame.TileIdentifier);
                output.Write(frame.Duration);
            }

            output.WriteTiledMapProperties(tilesetTile.Properties);
        }

        private static void WriteLayers(ContentWriter output, IReadOnlyCollection<TiledMapLayerContent> layers)
        {
            output.Write(layers.Count);
            foreach (var layer in layers)
                WriteLayer(output, layer);
        }

        private static void WriteLayer(ContentWriter output, TiledMapLayerContent layer)
        {
            output.Write((byte)layer.Type);

            output.Write(layer.Name ?? string.Empty);
            output.Write(layer.Visible);
            output.Write(layer.Opacity);
            output.Write(layer.OffsetX);
            output.Write(layer.OffsetY);

            output.WriteTiledMapProperties(layer.Properties);

            switch (layer.Type)
            {
                case TiledMapLayerType.ImageLayer:
                    WriteImageLayer(output, (TiledMapImageLayerContent)layer);
                    break;
                case TiledMapLayerType.TileLayer:
                    WriteTileLayer(output, (TiledMapTileLayerContent)layer);
                    break;
                case TiledMapLayerType.ObjectLayer:
                    WriteObjectLayer(output, (TiledMapObjectLayerContent)layer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer.Type));
            }

            if (layer.Type != TiledMapLayerType.ObjectLayer)
                WriteModels(output, layer.Models);
        }

        private static void WriteImageLayer(ContentWriter output, TiledMapImageLayerContent imageLayer)
        {
            var textureAssetName = Path.ChangeExtension(imageLayer.Image.Source, null);
            output.Write(textureAssetName);
            output.Write(new Vector2(imageLayer.X, imageLayer.Y));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WriteTileLayer(ContentWriter output, TiledMapTileLayerContent tileLayer)
        {
            output.Write(tileLayer.Width);
            output.Write(tileLayer.Height);

            output.Write(tileLayer.Tiles.Length);

            foreach (var tile in tileLayer.Tiles)
            {
                output.Write(tile.GlobalTileIdentifierWithFlags);
                output.Write(tile.X);
                output.Write(tile.Y);
            }
        }

        private static void WriteObjectLayer(ContentWriter output, TiledMapObjectLayerContent layer)
        {
            output.Write(ColorHelper.FromHex(layer.Color));
            output.Write((byte)layer.DrawOrder);

            output.Write(layer.Objects.Count);

            foreach (var @object in layer.Objects)
                WriteObject(output, @object);
        }


        private static void WriteObject(ContentWriter output, TiledMapObjectContent @object)
        {
            var type = GetObjectType(@object);

            output.Write((byte)type);

            output.Write(@object.Identifier);
            output.Write(@object.Name ?? string.Empty);
            output.Write(@object.Type ?? string.Empty);
            output.Write(@object.X);
            output.Write(@object.Y);
            output.Write(@object.Width);
            output.Write(@object.Height);
            output.Write(@object.Rotation);
            output.Write(@object.Visible);

            output.WriteTiledMapProperties(@object.Properties);

            switch (type)
            {
                case TiledMapObjectType.Rectangle:
                case TiledMapObjectType.Ellipse:
                    break;
                case TiledMapObjectType.Tile:
                    output.Write(@object.GlobalIdentifier);
                    break;
                case TiledMapObjectType.Polygon:
                    WritePolyPoints(output, @object.Polygon.Points);
                    break;
                case TiledMapObjectType.Polyline:
                    WritePolyPoints(output, @object.Polyline.Points);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WritePolyPoints(ContentWriter writer, string @string)
        {
            var stringPoints = @string.Split(' ');

            writer.Write(stringPoints.Length);

            foreach (var stringPoint in stringPoints)
            {
                var xy = stringPoint.Split(',');
                var x = float.Parse(xy[0], CultureInfo.InvariantCulture.NumberFormat);
                writer.Write(x);
                var y = float.Parse(xy[1], CultureInfo.InvariantCulture.NumberFormat);
                writer.Write(y);
            }
        }

        public static TiledMapObjectType GetObjectType(TiledMapObjectContent @object)
        {
            if (@object.GlobalIdentifier > 0)
                return TiledMapObjectType.Tile;

            if (@object.Ellipse != null)
                return TiledMapObjectType.Ellipse;

            if (@object.Polygon != null)
                return TiledMapObjectType.Polygon;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (@object.Polyline != null)
                return TiledMapObjectType.Polyline;

            return TiledMapObjectType.Rectangle;
        }

        private static void WriteModels(ContentWriter output, IReadOnlyCollection<TiledMapLayerModelContent> models)
        {
            output.Write(models.Count);

            var animatedModelsCount = models.Count(x => x is TiledMapLayerAnimatedModelContent);
            output.Write(animatedModelsCount);

            foreach (var model in models)
            {
                var animatedModel = model as TiledMapLayerAnimatedModelContent;
                if (animatedModel != null)
                {
                    output.Write(true);
                    WriteAnimatedModel(output, animatedModel);
                }
                else
                {
                    output.Write(false);
                    WriteModel(output, model);
                }
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WriteModel(ContentWriter output, TiledMapLayerModelContent model)
        {
            output.Write(model.LayerName);
            output.Write(model.TextureAssetName);

            var vertexCount = model.Vertices.Count;
            output.Write(vertexCount);
            foreach (var vertex in model.Vertices)
            {
                output.Write(vertex.Position.X);
                output.Write(vertex.Position.Y);
                output.Write(vertex.TextureCoordinate.X);
                output.Write(vertex.TextureCoordinate.Y);
            }

            var indexCount = model.Indices.Count;
            output.Write(indexCount);
            foreach (var index in model.Indices)
                output.Write(index);
        }

        private static void WriteAnimatedModel(ContentWriter output, TiledMapLayerAnimatedModelContent model)
        {
            WriteModel(output, model);

            output.Write(model.Tileset.FirstGlobalIdentifier);

            output.Write(model.AnimatedTilesetTiles.Count);
            foreach (var animatedTilesetTile in model.AnimatedTilesetTiles)
                output.Write(animatedTilesetTile.LocalIdentifier);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Tiled.TiledMap, MonoGame.Extended.Tiled";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Tiled.TiledMapReader, MonoGame.Extended.Tiled";
        }
    }
}