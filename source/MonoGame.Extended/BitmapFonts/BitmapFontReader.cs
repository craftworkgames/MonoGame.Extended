// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            var textureAssetCount = reader.ReadInt32();
            var assets = new List<string>();

            for (var i = 0; i < textureAssetCount; i++)
            {
                var assetName = reader.ReadString();
                assets.Add(assetName);
            }

            var textures = assets
                .Select(textureName => reader.ContentManager.Load<Texture2D>(reader.GetRelativeAssetName(textureName)))
                .ToArray();

            var face = reader.ReadString();
            var fontSize = reader.ReadInt16();
            var lineHeight = reader.ReadUInt16();
            var regionCount = reader.ReadInt32();
            var regions = new BitmapFontCharacter[regionCount];

            for (var r = 0; r < regionCount; r++)
            {
                var character = reader.ReadUInt32();
                var textureIndex = reader.ReadByte();
                var x = reader.ReadUInt16();
                var y = reader.ReadUInt16();
                var width = reader.ReadUInt16();
                var height = reader.ReadUInt16();
                var xOffset = reader.ReadInt16();
                var yOffset = reader.ReadInt16();
                var xAdvance = reader.ReadInt16();
                var textureRegion = new TextureRegion(textures[textureIndex], x, y, width, height);
                regions[r] = new BitmapFontCharacter((int)character, textureRegion, xOffset, yOffset, xAdvance);
            }

            var characterMap = regions.ToDictionary(r => r.Character);
            var kerningsCount = reader.ReadInt32();

            for (var k = 0; k < kerningsCount; k++)
            {
                var first = reader.ReadUInt32();
                var second = reader.ReadUInt32();
                var amount = reader.ReadInt16();

                // Find region
                if (!characterMap.TryGetValue((int)first, out var region))
                    continue;

                region.Kernings[(int)second] = amount;
            }

            return new BitmapFont(face, fontSize, lineHeight, regions);
        }
    }
}
