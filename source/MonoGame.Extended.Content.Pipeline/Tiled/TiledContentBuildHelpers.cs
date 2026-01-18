// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Extended.Content.Tiled;

#if MONOGAME_385_OR_NEWER && !KNI && !FNA
namespace MonoGame.Extended.Content.Pipeline.Tiled
{

    /// <summary>
    /// Provides helper methods for building eternal references in the Tiled content pipeline.
    /// </summary>
    public static class TiledContentBuildHelpers
    {
        /// <summary>
        /// Builds a texture external reference and stores it in the repository.
        /// </summary>
        /// <param name="repository">The external reference repository.</param>
        /// <param name="context">The processor context.</param>
        /// <param name="image">The image to build</param>
        public static void BuildTextureReference(IExternalReferenceRepository repository, ContentProcessorContext context, TiledMapImageContent image)
        {
            if (image?.Source is null)
                return;

            var parameters = new OpaqueDataDictionary
            {
                { "ColorKeyColor", image.TransparentColor },
                {"ColorKeyEnabled", true }
            };

            BuildTextureReference(repository, context, image.Source, parameters);
        }

        /// <summary>
        /// Builds a texture external reference with optional parameters.
        /// </summary>
        /// <param name="repository">The external reference repository.</param>
        /// <param name="context">The processor context.</param>
        /// <param name="source">The source texture path.</param>
        /// <param name="parameters">Optional processor parameters.</param>
        public static void BuildTextureReference(IExternalReferenceRepository repository, ContentProcessorContext context, string source, OpaqueDataDictionary parameters)
        {
            if (string.IsNullOrWhiteSpace(source))
                return;

            var textureImporter = new TextureImporter();
            var textureProcessor = new TextureProcessor();

            // Apply parameters if provided
            if (parameters != null)
            {
                if (parameters.TryGetValue("ColorKeyColor", out var colorKeyColor))
                    textureProcessor.ColorKeyColor = (Color)colorKeyColor;

                if (parameters.TryGetValue("ColorKeyEnabled", out var colorKeyEnabled))
                    textureProcessor.ColorKeyEnabled = (bool)colorKeyEnabled;

                if (parameters.TryGetValue("GenerateMipmaps", out var generateMipmaps))
                    textureProcessor.GenerateMipmaps = (bool)generateMipmaps;

                if (parameters.TryGetValue("PremultiplyAlpha", out var premultiplyAlpha))
                    textureProcessor.PremultiplyAlpha = (bool)premultiplyAlpha;

                if (parameters.TryGetValue("ResizeToPowerOfTwo", out var resizeToPowerOfTwo))
                    textureProcessor.ResizeToPowerOfTwo = (bool)resizeToPowerOfTwo;

                if (parameters.TryGetValue("TextureFormat", out var textureFormat))
                    textureProcessor.TextureFormat = (TextureProcessorOutputFormat)textureFormat;
            }

            var sourceAsset = new ExternalReference<TextureContent>(source);
            var externalReference = context.BuildAsset<TextureContent, TextureContent>(sourceAsset, textureImporter, textureProcessor, assetName: null);

            repository.StoreExternalReference(source, externalReference);
        }

        /// <summary>
        /// Builds a tileset external reference.
        /// </summary>
        /// <param name="repository">The external reference repository.</param>
        /// <param name="context">The processor context.</param>
        /// <param name="source">The source tileset path/</param>
        public static void BuildTilesetReference(IExternalReferenceRepository repository, ContentProcessorContext context, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return;

            var tilesetImporter = new TiledMapTilesetImporter();
            var tilesetProcessor = new TiledMapTilesetProcessor();

            var sourceAsset = new ExternalReference<TiledMapTilesetContentItem>(source);
            var externalReference = context.BuildAsset<TiledMapTilesetContentItem, TiledMapTilesetContentItem>(sourceAsset, tilesetImporter, tilesetProcessor, assetName: null);
        }

        /// <summary>
        /// Builds and loads an object template.
        /// </summary>
        /// <param name="context">The processor context.</param>
        /// <param name="source">The source template path.</param>
        /// <returns>The loaded template content.</returns>
        public static TiledMapObjectTemplateContent BuildAndLoadTemplate(ContentProcessorContext context, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var templateImporter = new TiledMapObjectTemplateImporter();
            var templateProcessor = new TiledMapObjectTemplateProcessor();

            var sourceAsset = new ExternalReference<TiledMapObjectTemplateContent>(source);
            var template = context.BuildAndLoadAsset<TiledMapObjectTemplateContent, TiledMapObjectTemplateContent>(sourceAsset, templateImporter, templateProcessor);

            return template;
        }
    }
}
#endif
