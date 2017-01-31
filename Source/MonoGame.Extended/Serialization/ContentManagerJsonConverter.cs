using System;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    /// <summary>
    /// Loads content from a JSON file into the <see cref="ContentManager"/> using the asset name
    /// </summary>
    /// <typeparam name="T">The type of content to load</typeparam>
    public class ContentManagerJsonConverter<T> : JsonConverter
    {
        private readonly ContentManager _contentManager;
        private readonly Func<T, string> _getAssetName;

        public ContentManagerJsonConverter(ContentManager contentManager, Func<T, string> getAssetName)
        {
            _contentManager = contentManager;
            _getAssetName = getAssetName;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var asset = (T)value;
            var assetName = _getAssetName(asset);
            writer.WriteValue(assetName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var assetName = (string)reader.Value;
            return _contentManager.Load<T>(assetName);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}