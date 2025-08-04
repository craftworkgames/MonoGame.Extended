// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Extended.Content.TexturePacker;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases;

[ContentProcessor(DisplayName = "TexturePacker Processor - MonoGame.Extended")]
public class TexturePackerProcessor : ContentProcessor<TexturePackerFileContent, TexturePackerProcessorResult>
{
    public override TexturePackerProcessorResult Process(TexturePackerFileContent input, ContentProcessorContext context)
    {
        if (input.Meta.Image != null)
        {
            // Validates the texture exists and can be processed (fails build if missing)
            var externalRef = new ExternalReference<Texture2DContent>(input.Meta.Image);
            context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(externalRef, nameof(TextureProcessor));

        }
        else if (input.Meta.DataFormat == "monogame-extended")
        {
            foreach (var texture in input.Textures)
            {
                var externalRef = new ExternalReference<Texture2DContent>(texture.FileName);
                context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(externalRef, nameof(TextureProcessor));
            }
        }
        return new TexturePackerProcessorResult(input);
    }
}
