﻿using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentProcessor(DisplayName = "TexturePacker Processor - MonoGame.Extended")]
    public class TexturePackerProcessor : ContentProcessor<TexturePackerFile, TexturePackerProcessorResult>
    {
        public override TexturePackerProcessorResult Process(TexturePackerFile input, ContentProcessorContext context)
        {
            try
            {
                var output = new TexturePackerProcessorResult
                {
                    Data = input
                };
                return output;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}