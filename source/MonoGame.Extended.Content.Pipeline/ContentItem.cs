using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Extended.Content.Tiled;
using MonoGame.Extended.Content.Pipeline.Tiled;

namespace MonoGame.Extended.Content.Pipeline
{
    public interface IExternalReferenceRepository
    {
        ExternalReference<TInput> GetExternalReference<TInput>(string source);
    }

    public class ContentItem<T> : ContentItem, IExternalReferenceRepository
    {
        public ContentItem(T data)
        {
            Data = data;
        }

        public T Data { get; }

        private readonly Dictionary<string, ContentItem> _externalReferences = new Dictionary<string, ContentItem>();

        public void BuildExternalReference<TInput>(ContentProcessorContext context, string source, OpaqueDataDictionary parameters = null)
        {
            var sourceAsset = new ExternalReference<TInput>(source);

#if MONOGAME_385_OR_NEWER
            // MonoGame 3.8.5+ uses the new API with importer/processor instances
            var (importer, processor) = GetImporterAndProcessor<TInput>(parameters);
            var externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, importer, processor, null);
#else
            // MonoGame 3.8.4 uses the old API with string names
            var externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, "", parameters, "", "");
#endif
            _externalReferences.Add(source, externalReference);
        }

#if MONOGAME_385_OR_NEWER
        private (IContentImporter, IContentProcessor) GetImporterAndProcessor<TInput>(OpaqueDataDictionary parameters)
        {
            var inputType = typeof(TInput);

            if (inputType == typeof(Texture2DContent))
            {
                var importer = new TextureImporter();
                var processor = new TextureProcessor();

                if (parameters != null)
                {
                    ApplyTextureProcessorParameters(processor, parameters);
                }

                return (importer, processor);
            }
            else if(inputType == typeof(TiledMapTilesetContent))
            {
                var importer = new TiledMapTilesetImporter();
                var processor = new TiledMapTilesetProcessor();

                return (importer, processor);
            }

            // Add support for other content types as discovered
            throw new NotSupportedException(
                $"No default importer/processor mapping for content type '{inputType.Name}'. " +
                $"Please open an issue at https://github.com/monogame-extended/monogame-extended/issues"
            );
        }

        private void ApplyTextureProcessorParameters(TextureProcessor processor, OpaqueDataDictionary parameters)
        {
            if (parameters.TryGetValue(nameof(TextureProcessor.ColorKeyColor), out var colorKeyColor))
            {
                processor.ColorKeyColor = (Color)colorKeyColor;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.ColorKeyEnabled), out var colorKeyEnabled))
            {
                processor.ColorKeyEnabled = (bool)colorKeyEnabled;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.GenerateMipmaps), out var generateMipMaps))
            {
                processor.GenerateMipmaps = (bool)generateMipMaps;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.PremultiplyAlpha), out var preMultiplyAlpha))
            {
                processor.PremultiplyAlpha = (bool)preMultiplyAlpha;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.ResizeToPowerOfTwo), out var resizePowerOfTwo))
            {
                processor.ResizeToPowerOfTwo = (bool)resizePowerOfTwo;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.MakeSquare), out var makeSquare))
            {
                processor.MakeSquare = (bool)makeSquare;
            }

            if (parameters.TryGetValue(nameof(TextureProcessor.TextureFormat), out var textureFormat))
            {
                processor.TextureFormat = (TextureProcessorOutputFormat)textureFormat;
            }
        }
#endif

        public ExternalReference<TInput> GetExternalReference<TInput>(string source)
        {
            if (source is not null && _externalReferences.TryGetValue(source, out var contentItem))
                return contentItem as ExternalReference<TInput>;

            return null;
        }
    }
}
