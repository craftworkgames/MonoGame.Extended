using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Content
{
    /// <summary>
    /// Reads a binary tileset asset produced by the content pipeline into a <see cref="TilemapTileset"/>.
    /// </summary>
    public sealed class TilemapTilesetReader : ContentTypeReader<TilemapTileset>
    {
        internal static readonly string NativeAotRegistrationKey =
            $"{typeof(TilemapTilesetReader).FullName}, {typeof(TilemapTilesetReader).Assembly.GetName().Name}";

#if !FNA && !KNI
        /// <summary>
        /// Registers this <see cref="ContentTypeReader"/> with the <see cref="ContentTypeReaderManager"/>
        /// so it is resolved without reflection.
        /// </summary>
        /// <remarks>
        /// Call this method once during application startup when publishing with
        /// <c>PublishAot</c> or <c>PublishTrimmed</c>.
        /// </remarks>
        public static void Register() =>
            ContentTypeReaderManager.AddTypeCreator(
                NativeAotRegistrationKey,
                () => new TilemapTilesetReader());
#endif

        /// <inheritdoc/>
        protected override TilemapTileset Read(ContentReader reader, TilemapTileset existingInstance)
        {
            ArgumentNullException.ThrowIfNull(reader);
            return ReadTileset(reader);
        }

        /// <summary>
        /// Reads a tileset. Called from both this reader and <see cref="TilemapReader"/>
        /// for inline tilesets.
        /// </summary>
        internal static TilemapTileset ReadTileset(ContentReader reader)
        {
            string name = reader.ReadString();
            Texture2D texture = reader.ReadExternalReference<Texture2D>();

            int tileWidth = reader.ReadInt32();
            int tileHeight = reader.ReadInt32();
            int tileCount = reader.ReadInt32();
            int columns = reader.ReadInt32();
            int spacing = reader.ReadInt32();
            int margin = reader.ReadInt32();

            float offsetX = reader.ReadSingle();
            float offsetY = reader.ReadSingle();

            TilemapTileset tileset = new TilemapTileset(
                name, texture, tileWidth, tileHeight, tileCount, columns, spacing, margin);
            tileset.TileOffset = new Vector2(offsetX, offsetY);

            ReadProperties(reader, tileset.Properties);
            ReadTileDataEntries(reader, tileset);

            return tileset;
        }

        internal static void ReadProperties(ContentReader reader, TilemapProperties properties)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                byte type = reader.ReadByte();

                switch (type)
                {
                    case 0: // String
                        properties.SetString(key, reader.ReadString());
                        break;
                    case 1: // Int
                        properties.SetInt(key, reader.ReadInt32());
                        break;
                    case 2: // Float
                        properties.SetFloat(key, reader.ReadSingle());
                        break;
                    case 3: // Bool
                        properties.SetBool(key, reader.ReadBoolean());
                        break;
                    case 4: // Color
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();
                        byte a = reader.ReadByte();
                        properties.SetColor(key, new Color(r, g, b, a));
                        break;
                    case 5: // File
                        properties.SetString(key, reader.ReadString());
                        break;
                    default:
                        properties.SetString(key, reader.ReadString());
                        break;
                }
            }
        }

        private static void ReadTileDataEntries(ContentReader reader, TilemapTileset tileset)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                TilemapTileData tileData = ReadTileData(reader);
                tileset.AddTileData(tileData);
            }
        }

        private static TilemapTileData ReadTileData(ContentReader reader)
        {
            int localId = reader.ReadInt32();
            string cls = reader.ReadString();
            float prob = reader.ReadSingle();

            TilemapTileData tileData = new TilemapTileData(localId);
            tileData.Class = cls;
            tileData.Probability = prob;

            ReadProperties(reader, tileData.Properties);

            bool hasAnimation = reader.ReadBoolean();

            if (hasAnimation)
            {
                tileData.Animation = ReadAnimation(reader);
            }

            int collisionCount = reader.ReadInt32();

            for (int i = 0; i < collisionCount; i++)
            {
                TilemapObject obj = TilemapObjectReader.ReadObject(reader);
                tileData.CollisionObjects.Add(obj);
            }

            bool hasTileImage = reader.ReadBoolean();

            if (hasTileImage)
            {
                tileData.CustomImage = reader.ReadExternalReference<Texture2D>();
            }

            return tileData;
        }

        private static TilemapTileAnimation ReadAnimation(ContentReader reader)
        {
            int frameCount = reader.ReadInt32();
            TilemapTileAnimationFrame[] frames = new TilemapTileAnimationFrame[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                int tileId = reader.ReadInt32();
                float duration = reader.ReadSingle();
                frames[i] = new TilemapTileAnimationFrame(tileId, duration);
            }

            return new TilemapTileAnimation(frames);
        }
    }
}
