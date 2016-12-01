using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiTextureAtlasConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public GuiTextureAtlasConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            //var assetName = reader.Value.ToString();

            //using (var stream = TitleContainer.OpenStream(assetName))
            //{
            //    return TextureAtlasReader.FromRawXml(_contentManager, stream);
            //}
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureAtlas);
        }
    }
}