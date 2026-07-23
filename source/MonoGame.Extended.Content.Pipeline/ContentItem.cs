using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content.Pipeline;

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
            ExternalReference<TInput> sourceAsset = new ExternalReference<TInput>(source);
            ExternalReference<TInput> externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, "", parameters, "", "");
            _externalReferences.Add(source, externalReference);
        }

#if !KNI && !FNA
        /// <summary>
        /// Builds an external asset reference using explicit importer and processor instances.
        /// </summary>
        /// <typeparam name="TInput">The importer output type for the source asset.</typeparam>
        /// <typeparam name="TOutput">The reference type to store for the built asset.</typeparam>
        /// <param name="context">The active content processor context.</param>
        /// <param name="source">The source asset path.</param>
        /// <param name="importer">The importer to use for the nested build.</param>
        /// <param name="processor">The processor to use for the nested build.</param>
        public void BuildExternalReference<TInput, TOutput>(
            ContentProcessorContext context,
            string source,
            IContentImporter importer,
            IContentProcessor processor)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(importer);
            ArgumentNullException.ThrowIfNull(processor);

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Source path cannot be null or whitespace.", nameof(source));
            }

            ExternalReference<TInput> sourceAsset = new ExternalReference<TInput>(source);
            ExternalReference<TOutput> externalReference =
                context.BuildAsset<TInput, TOutput>(sourceAsset, importer, processor);
            _externalReferences.Add(source, externalReference);
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
