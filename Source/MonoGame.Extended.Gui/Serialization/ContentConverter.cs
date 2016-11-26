using System;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class ContentConverter<T> : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public ContentConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var assetName = serializer.Deserialize<JToken>(reader).Value<string>();
            return _contentManager.Load<T>(assetName);
        }
    }
}