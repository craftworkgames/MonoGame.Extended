using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline
{
#if MONOGAME_385_OR_NEWER && !KNI && !FNA
    /// <summary>
    /// Defines a repository for storing and retrieving external references.
    /// </summary>
    public interface IExternalReferenceRepository
    {
        /// <summary>
        /// Retrieves a previously stored external reference by source path.
        /// </summary>
        /// <typeparam name="TInput">The type of the external reference.</typeparam>
        /// <param name="source">The source path of the asset.</param>
        /// <returns>The external reference, or null if not found.</returns>
        ExternalReference<TInput> GetExternalReference<TInput>(string source);

        /// <summary>
        /// Stores an external reference for later retrieval.
        /// </summary>
        /// <typeparam name="TInput">The type of the external reference.</typeparam>
        /// <param name="source">The source path of the asset.</param>
        /// <param name="reference">The external reference to store.</param>
        void StoreExternalReference<TInput>(string source, ExternalReference<TInput> reference);
    }

    /// <summary>
    /// Base content item class that provides external reference storage capability.
    /// </summary>
    /// <typeparam name="T">The type of data this content item contains.</typeparam>
    public class ContentItem<T> : ContentItem, IExternalReferenceRepository
    {
        private readonly Dictionary<string, ContentItem> _externalReferences = new Dictionary<string, ContentItem>();

        /// <summary>
        /// Gets the content data.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentItem{T}"/> class.
        /// </summary>
        /// <param name="data">The content data.</param>
        public ContentItem(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Retrieves a previously stored external reference by source path.
        /// </summary>
        /// <typeparam name="TInput">The type of the external reference.</typeparam>
        /// <param name="source">The source path of the asset.</param>
        /// <returns>The external reference, or null if not found.</returns>
        public ExternalReference<TInput> GetExternalReference<TInput>(string source)
        {
            if (source is not null && _externalReferences.TryGetValue(source, out var reference))
                return reference as ExternalReference<TInput>;

            return null;
        }

        /// <summary>
        /// Stores an external reference for later retrieval.
        /// </summary>
        /// <typeparam name="TInput">The type of external reference.</typeparam>
        /// <param name="source">The source path of the asset.</param>
        /// <param name="reference">The external reference to store.</param>
        public void StoreExternalReference<TInput>(string source, ExternalReference<TInput> reference)
        {
            if (source is not null && reference is not null)
                _externalReferences[source] = reference;
        }

        [Obsolete]
        public void BuildExternalReference<TInput>(ContentProcessorContext context, string source, OpaqueDataDictionary parameters = null)
        {
            var sourceAsset = new ExternalReference<TInput>(source);
            var externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, "", parameters, "", "");
            _externalReferences.Add(source, externalReference);
        }
    }
#else

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
            var externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, "", parameters, "", "");
            _externalReferences.Add(source, externalReference);
        }

        public ExternalReference<TInput> GetExternalReference<TInput>(string source)
        {
            if (source is not null && _externalReferences.TryGetValue(source, out var contentItem))
                return contentItem as ExternalReference<TInput>;

            return null;
        }
    }
#endif
}
