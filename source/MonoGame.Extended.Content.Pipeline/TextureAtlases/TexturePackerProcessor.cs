// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Extended.Content.TexturePacker;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases;

[ContentProcessor(DisplayName = "TexturePacker Processor - MonoGame.Extended")]
public class TexturePackerProcessor : ContentProcessor<ContentImporterResult<TexturePackerFileContent>, TexturePackerProcessorResult>
{
    public override TexturePackerProcessorResult Process(ContentImporterResult<TexturePackerFileContent> input, ContentProcessorContext context)
    {
        if (input.Data.Meta.Image != null)
        {
            // Validates the texture exists and can be processed (fails build if missing)
            ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(input.Data.Meta.Image);
            context.BuildAndLoadAsset<TextureContent, TextureContent>(
                externalRef,
                new TextureImporter(),
                new TextureProcessor());

        }
        else if (input.Data.Meta.DataFormat == "monogame-extended")
        {
            foreach (TexturePackerTexture texture in input.Data.Textures)
            {
                string texturePath = Path.Combine(Path.GetDirectoryName(input.FilePath), texture.FileName);
                ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(texturePath);
                context.BuildAndLoadAsset<TextureContent, TextureContent>(
                    externalRef,
                    new TextureImporter(),
                    new TextureProcessor());
            }
        }
        return new TexturePackerProcessorResult(input.Data);
    }
}
