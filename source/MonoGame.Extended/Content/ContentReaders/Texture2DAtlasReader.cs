// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.ContentReaders
{
    public class Texture2DAtlasReader : ContentTypeReader<Texture2DAtlas>
    {
        protected override Texture2DAtlas Read(ContentReader reader, Texture2DAtlas existingInstance)
        {
            var imageAssetName = reader.ReadString();
            var texture = reader.ContentManager.Load<Texture2D>(reader.GetRelativeAssetName(imageAssetName));
            var atlas = new Texture2DAtlas(imageAssetName, texture);

            var regionCount = reader.ReadInt32();

            for (var i = 0; i < regionCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                string regionName = reader.ReadString();
                atlas.CreateRegion(x, y, width, height, regionName);
            }

            return atlas;
        }
    }
}
