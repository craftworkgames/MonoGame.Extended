using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tiled;
using System;
using System.Globalization;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[ContentTypeWriter]
	public class TiledMapTilesetWriter : ContentTypeWriter<TiledMapTilesetContentItem>
	{
		public override string GetRuntimeReader(TargetPlatform targetPlatform) => "MonoGame.Extended.Tiled.TiledMapTilesetReader, MonoGame.Extended.Tiled";

	    public override string GetRuntimeType(TargetPlatform targetPlatform) => "MonoGame.Extended.Tiled.TiledMapTileset, MonoGame.Extended.Tiled";

		protected override void Write(ContentWriter writer, TiledMapTilesetContentItem contentItem)
		{
			try
			{
				WriteTileset(writer, contentItem.Data, contentItem);
			}
			catch (Exception ex)
			{
                ContentLogger.Logger.LogImportantMessage(ex.StackTrace);
                throw;
			}
		}

		public static void WriteTileset(ContentWriter writer, TiledMapTilesetContent tileset, IExternalReferenceRepository externalReferenceRepository)
		{
		    var externalReference = externalReferenceRepository.GetExternalReference<Texture2DContent>(tileset.Image.Source);

			writer.WriteExternalReference(externalReference);
            writer.Write(tileset.TileWidth);
            writer.Write(tileset.TileHeight);
            writer.Write(tileset.TileCount);
            writer.Write(tileset.Spacing);
            writer.Write(tileset.Margin);
            writer.Write(tileset.Columns);
            writer.Write(tileset.Tiles.Count);

            foreach (var tilesetTile in tileset.Tiles)
                WriteTilesetTile(writer, tilesetTile);

            writer.WriteTiledMapProperties(tileset.Properties);
		}

        private static void WriteTilesetTile(ContentWriter writer, TiledMapTilesetTileContent tilesetTile)
        {
            writer.Write(tilesetTile.LocalIdentifier);
            writer.Write(tilesetTile.Type);
            writer.Write(tilesetTile.Frames.Count);
            writer.Write(tilesetTile.Objects.Count);

            foreach (var @object in tilesetTile.Objects)
                WriteObject(writer, @object);

            foreach (var frame in tilesetTile.Frames)
            {
                writer.Write(frame.TileIdentifier);
                writer.Write(frame.Duration);
            }

            writer.WriteTiledMapProperties(tilesetTile.Properties);
        }

        private static void WriteObject(ContentWriter writer, TiledMapObjectContent @object)
        {
            var type = GetObjectType(@object);

            writer.Write((byte)type);

            writer.Write(@object.Identifier);
            writer.Write(@object.Name ?? string.Empty);
            writer.Write(@object.Type ?? string.Empty);
            writer.Write(@object.X);
            writer.Write(@object.Y);
            writer.Write(@object.Width);
            writer.Write(@object.Height);
            writer.Write(@object.Rotation);
            writer.Write(@object.Visible);

            writer.WriteTiledMapProperties(@object.Properties);

            switch (type)
            {
                case TiledMapObjectType.Rectangle:
                case TiledMapObjectType.Ellipse:
                    break;
                case TiledMapObjectType.Tile:
                    writer.Write(@object.GlobalIdentifier);
                    break;
                case TiledMapObjectType.Polygon:
                    WritePolyPoints(writer, @object.Polygon.Points);
                    break;
                case TiledMapObjectType.Polyline:
                    WritePolyPoints(writer, @object.Polyline.Points);
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

        public static TiledMapObjectType GetObjectType(TiledMapObjectContent content)
        {
            if (content.GlobalIdentifier > 0)
                return TiledMapObjectType.Tile;

            if (content.Ellipse != null)
                return TiledMapObjectType.Ellipse;

            if (content.Polygon != null)
                return TiledMapObjectType.Polygon;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (content.Polyline != null)
                return TiledMapObjectType.Polyline;

            return TiledMapObjectType.Rectangle;
        }
	}
}
