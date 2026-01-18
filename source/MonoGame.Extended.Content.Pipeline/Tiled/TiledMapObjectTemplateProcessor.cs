// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Tiled;

#if MONOGAME_385_OR_NEWER && !KNI && !FNA
namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    /// <summary>
    /// Processes Tiled object template files.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Map Object Template Processor - MonoGame.Extended")]
    public class TiledMapObjectTemplateProcessor : ContentProcessor<TiledMapObjectTemplateContent, TiledMapObjectTemplateContent>
    {
        /// <summary>
        /// Processes the object template content.
        /// </summary>
        /// <param name="input">The template content to process.</param>
        /// <param name="context">The processor context.</param>
        /// <returns>The processed template content.</returns>
        public override TiledMapObjectTemplateContent Process(TiledMapObjectTemplateContent input, ContentProcessorContext context)
        {
            ContentLogger.Logger = context.Logger;

            // Object templates are simple pass-through, the just need to be serialized.
            // The actual processing happens when they're loaded and applied to objects
            return input;
        }
    }
}
#endif
