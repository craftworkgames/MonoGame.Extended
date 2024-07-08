// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.TexturePacker;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases;

[ContentProcessor(DisplayName = "TexturePacker Processor - MonoGame.Extended")]
public class TexturePackerProcessor : ContentProcessor<TexturePackerFileContent, TexturePackerProcessorResult>
{
    public override TexturePackerProcessorResult Process(TexturePackerFileContent input, ContentProcessorContext context)
    {
        return new TexturePackerProcessorResult(input);
    }
}
