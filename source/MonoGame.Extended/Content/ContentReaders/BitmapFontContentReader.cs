// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content.BitmapFonts;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.ContentReaders;

public class BitmapFontContentReader : ContentTypeReader<BitmapFont>
{
    protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
    {
        if (existingInstance is not null)
        {
            return existingInstance;
        }

        var bmfFile = BitmapFontFileReader.Read((FileStream)reader.BaseStream);

        var textures =
            bmfFile.Pages.Select(page => reader.ContentManager.Load<Texture2D>(reader.GetRelativeAssetName(page)))
            .ToArray();

        var characters = new Dictionary<int, BitmapFontCharacter>();
        foreach (var charBlock in bmfFile.Characters)
        {
            var texture = textures[charBlock.Page];
            var region = new Texture2DRegion(texture, charBlock.X, charBlock.Y, charBlock.Width, charBlock.Height);
            var character = new BitmapFontCharacter((int)charBlock.ID, region, charBlock.XOffset, charBlock.YOffset, charBlock.XAdvance);
            characters.Add(character.Character, character);
        }


        foreach (var kerningBlock in bmfFile.Kernings)
        {
            if (characters.TryGetValue((int)kerningBlock.First, out var character))
            {
                character.Kernings.Add((int)kerningBlock.Second, kerningBlock.Amount);
            }
        }

        return new BitmapFont(bmfFile.FontName, bmfFile.Info.FontSize, bmfFile.Common.LineHeight, characters.Values);
    }
}
