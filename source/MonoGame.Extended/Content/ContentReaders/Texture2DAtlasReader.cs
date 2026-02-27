// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.ContentReaders
{
    public class Texture2DAtlasReader : ContentTypeReader<Texture2DAtlas>
    {
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
                typeof(Texture2DAtlasReader).AssemblyQualifiedName,
                () => new Texture2DAtlasReader());
#endif

        protected override Texture2DAtlas Read(ContentReader reader, Texture2DAtlas existingInstance)
        {
            var imageAssetName = reader.ReadString();
            var texture = reader.ContentManager.Load<Texture2D>(reader.GetRelativeAssetName(imageAssetName));
            var atlas = new Texture2DAtlas(reader.AssetName, texture);

            var regionCount = reader.ReadInt32();

            for (var i = 0; i < regionCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                string regionName = reader.ReadString();

                int rotated = reader.ReadInt32();

                int origWidth, origHeight, offsetX, offsetY;
                if (reader.ReadBoolean())  // trimmed sprite?
                {
                    origWidth = reader.ReadInt32();
                    origHeight = reader.ReadInt32();
                    offsetX = reader.ReadInt32();
                    offsetY = reader.ReadInt32();
                }
                else
                {
                    origWidth = (rotated == 0) ? width : height;
                    origHeight = (rotated == 0) ? height : width;
                    offsetX = offsetY = 0;
                }

                Vector2? pivotPoint = null;
                if (reader.ReadBoolean())  // pivot point available?
                {
                    float ppX = (float)reader.ReadDouble();
                    float ppY = (float)reader.ReadDouble();
                    pivotPoint = new Vector2(ppX, ppY);
                }

                atlas.CreateRegion(new Rectangle(x, y, width, height),
                                   rotated != 0,
                                   new Size(origWidth, origHeight),
                                   new Vector2(offsetX, offsetY),
                                   pivotPoint,
                                   regionName);
            }

            return atlas;
        }
    }
}
