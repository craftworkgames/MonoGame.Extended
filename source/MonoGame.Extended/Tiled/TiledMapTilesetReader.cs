using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using System;

namespace MonoGame.Extended.Tiled
{
	public class TiledMapTilesetReader : ContentTypeReader<TiledMapTileset>
	{
		protected override TiledMapTileset Read(ContentReader reader, TiledMapTileset existingInstance)
		{
			if (existingInstance != null)
				return existingInstance;

			return ReadTileset(reader);
		}

		public static TiledMapTileset ReadTileset(ContentReader reader)
		{
			var texture = reader.ReadExternalReference<Texture2D>();
            var @class = reader.ReadString();
            var tileWidth = reader.ReadInt32();
            var tileHeight = reader.ReadInt32();
            var tileCount = reader.ReadInt32();
            var spacing = reader.ReadInt32();
            var margin = reader.ReadInt32();
            var columns = reader.ReadInt32();
            var explicitTileCount = reader.ReadInt32();

            var tileset = new TiledMapTileset(texture, @class, tileWidth, tileHeight, tileCount, spacing, margin, columns);

            for (var tileIndex = 0; tileIndex < explicitTileCount; tileIndex++)
                ReadTile(reader, tileset);

            reader.ReadTiledMapProperties(tileset.Properties);
            return tileset;
		}

        private static void ReadTile(ContentReader reader, TiledMapTileset tileset)
        {
            var texture = reader.ReadExternalReference<Texture2D>();

            var localTileIdentifier = reader.ReadInt32();
            var type = reader.ReadString();
            var animationFramesCount = reader.ReadInt32();
            var objectCount = reader.ReadInt32();
            var objects = new TiledMapObject[objectCount];

            for (var i = 0; i < objectCount; i++)
                objects[i] = ReadTiledMapObject(reader, tileset);

            var tilesetTile = animationFramesCount <= 0
                ? new TiledMapTilesetTile(localTileIdentifier, type, objects, texture)
                : new TiledMapTilesetAnimatedTile(localTileIdentifier,
                    ReadTiledMapTilesetAnimationFrames(reader, tileset, animationFramesCount), type, objects, texture);

            reader.ReadTiledMapProperties(tilesetTile.Properties);
            tileset.Tiles.Add(tilesetTile);
        }

        private static TiledMapTilesetTileAnimationFrame[] ReadTiledMapTilesetAnimationFrames(ContentReader reader, TiledMapTileset tileset, int animationFramesCount)
		{
			var animationFrames = new TiledMapTilesetTileAnimationFrame[animationFramesCount];

			for (var i = 0; i < animationFramesCount; i++)
			{
				var localTileIdentifierForFrame = reader.ReadInt32();
				var frameDurationInMilliseconds = reader.ReadInt32();
				var tileSetTileFrame = new TiledMapTilesetTileAnimationFrame(tileset, localTileIdentifierForFrame, frameDurationInMilliseconds);
				animationFrames[i] = tileSetTileFrame;
			}

			return animationFrames;
		}

		private static TiledMapTilesetTile ReadTiledMapTilesetTile(ContentReader reader, TiledMapTileset tileset, Func<TiledMapObject[], TiledMapTilesetTile> createTile)
		{
            var texture = reader.ReadExternalReference<Texture2D>();
			var objectCount = reader.ReadInt32();
			var objects = new TiledMapObject[objectCount];

			for (var i = 0; i < objectCount; i++)
				objects[i] = ReadTiledMapObject(reader, tileset);

			return createTile(objects);
		}

		private static TiledMapObject ReadTiledMapObject(ContentReader reader, TiledMapTileset tileset)
		{
			var objectType = (TiledMapObjectType)reader.ReadByte();
			var identifier = reader.ReadInt32();
			var name = reader.ReadString();
			var type = reader.ReadString();
			var position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			var width = reader.ReadSingle();
			var height = reader.ReadSingle();
			var size = new SizeF(width, height);
			var rotation = reader.ReadSingle();
			var isVisible = reader.ReadBoolean();
			var properties = new TiledMapProperties();
			const float opacity = 1.0f;

            reader.ReadTiledMapProperties(properties);

			TiledMapObject mapObject;

			switch (objectType)
			{
				case TiledMapObjectType.Rectangle:
					mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Tile:
					reader.ReadUInt32(); // Tile objects within TiledMapTilesetTiles currently ignore the gid and behave like rectangle objects.
					mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Ellipse:
					mapObject = new TiledMapEllipseObject(identifier, name, size, position, rotation, opacity, isVisible);
					break;
				case TiledMapObjectType.Polygon:
					mapObject = new TiledMapPolygonObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Polyline:
					mapObject = new TiledMapPolylineObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			foreach (var property in properties)
				mapObject.Properties.Add(property.Key, property.Value);

			return mapObject;
		}

		private static Vector2[] ReadPoints(ContentReader reader)
		{
			var pointCount = reader.ReadInt32();
			var points = new Vector2[pointCount];

			for (var i = 0; i < pointCount; i++)
			{
				var x = reader.ReadSingle();
				var y = reader.ReadSingle();
				points[i] = new Vector2(x, y);
			}

			return points;
		}
	}
}
