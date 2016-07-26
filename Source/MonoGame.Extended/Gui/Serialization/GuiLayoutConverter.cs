using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiLayoutConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public GuiLayoutConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiLayout);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            //var styleSheet = LoadStyleSheet(serializer, jObject);
            var textureAtlas = jObject.Property(nameof(GuiLayout.TextureAtlas)).Value.ToObject<TextureAtlas>(serializer);

            serializer.Converters.Add(new TextureRegion2DConveter(textureAtlas));
            serializer.Converters.Add(new GuiControlConverter());
            
            
            return new GuiLayout
            {
                //StyleSheet = styleSheet,
                TextureAtlas = textureAtlas,
                Controls = jObject.Property(nameof(GuiLayout.Controls)).Value.ToObject<GuiControl[]>(serializer)
            };
        }

        //private GuiStyleSheet LoadStyleSheet(JsonSerializer serializer, JObject jObject)
        //{
        //    var styleSheetName = jObject.Property(nameof(GuiLayout.StyleSheet)).Value.ToObject<string>();
        //    var styleSheetPath = Path.Combine(_contentManager.RootDirectory, styleSheetName);

        //    using (var stream = TitleContainer.OpenStream(styleSheetPath))
        //    using (var streamReader = new StreamReader(stream))
        //    {
        //        var json = streamReader.ReadToEnd();
        //        return JsonConvert.DeserializeObject<GuiStyleSheet>(json, serializer.Converters.ToArray());
        //    }
        //}
    }
}