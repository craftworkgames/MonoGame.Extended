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
#if KNI || FNA
            // KNI and FNA do not use the new external ref calls from MonoGame's new
            // content builder project
            var externalRef = new ExternalReference<Texture2DContent>(input.Data.Meta.Image);
            context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(externalRef, nameof(TextureProcessor));
#else            
            ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(input.Data.Meta.Image);
            context.BuildAndLoadAsset<TextureContent, TextureContent>(
                externalRef,
                new TextureImporter(),
                new TextureProcessor());
#endif

        }
        else if (input.Data.Meta.DataFormat == "monogame-extended")
        {
            foreach (TexturePackerTexture texture in input.Data.Textures)
            {
                string texturePath = Path.Combine(Path.GetDirectoryName(input.FilePath), texture.FileName);

#if KNI || FNA
                // KNI and FNA do not use the new external ref calls from MonoGame's new
                // content builder project
                var externalRef = new ExternalReference<Texture2DContent>(texturePath);
                context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(externalRef, nameof(TextureProcessor));
#else                
                ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(texturePath);
                context.BuildAndLoadAsset<TextureContent, TextureContent>(
                externalRef,
                    new TextureImporter(),
                    new TextureProcessor());
#endif
            }
        }
        return new TexturePackerProcessorResult(input.Data);
    }
}
