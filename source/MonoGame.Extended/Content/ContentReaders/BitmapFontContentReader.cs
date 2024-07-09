// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.ContentReaders;

public class BitmapFontContentReader : ContentTypeReader<BitmapFont>
{
    protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
    {
        var textureCount = reader.ReadInt32();
        var textures = new Texture2D[textureCount];

        for (int i = 0; i < textureCount; i++)
        {
            var textureName = reader.ReadString();
            textures[i] = reader.ContentManager.Load<Texture2D>(reader.GetRelativeAssetName(textureName));
        }

        var fontName = reader.ReadString();
        var fontSize = reader.ReadInt16();
        var lineHeight = reader.ReadUInt16();

        var characterCount = reader.ReadInt32();
        var characters = new Dictionary<int, BitmapFontCharacter>();

        for (int i = 0; i < characterCount; i++)
        {
            var id = reader.ReadUInt32();
            var page = reader.ReadByte();
            var x = reader.ReadUInt16();
            var y = reader.ReadUInt16();
            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var xOffset = reader.ReadInt16();
            var yOffset = reader.ReadInt16();
            var xAdvance = reader.ReadInt16();

            var characterRegion = new Texture2DRegion(textures[page], x, y, width, height);
            var character = new BitmapFontCharacter((char)id, characterRegion, xOffset, yOffset, xAdvance);
            characters.Add(character.Character, character);
        }

        var kerningCount = reader.ReadInt32();

        for (int i = 0; i < kerningCount; i++)
        {
            var first = reader.ReadUInt32();
            var second = reader.ReadUInt32();
            var amount = reader.ReadInt16();

            if (characters.TryGetValue((int)first, out var character))
            {
                character.Kernings.Add((int)second, amount);
            }
        }

        return new BitmapFont(fontName, fontSize, lineHeight, characters.Values);
    }
}
