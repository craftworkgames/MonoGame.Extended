using System.Collections.Generic;
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
            var sourceAsset = new ExternalReference<TInput>(source);
            var externalReference = context.BuildAsset<TInput, TInput>(sourceAsset, "", parameters, "", "");
            _externalReferences.Add(source, externalReference);
        }

        public ExternalReference<TInput> GetExternalReference<TInput>(string source)
        {
            if (_externalReferences.TryGetValue(source, out var contentItem))
                return contentItem as ExternalReference<TInput>;

            return null;
        }
    }
}