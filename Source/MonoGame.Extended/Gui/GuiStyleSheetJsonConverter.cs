using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Wip;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui
{
    public class GuiStyleSheetJsonConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;

        public GuiStyleSheetJsonConverter(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var styleSheet = new GuiStyleSheet()
            {
                Fonts = jObject.Property(nameof(GuiStyleSheet.Fonts)).Value.ToObject<string[]>(),
                TextureAtlas = jObject.Property(nameof(GuiStyleSheet.TextureAtlas)).Value.ToObject<string>()
            };

            var bitmapFonts = styleSheet.Fonts
                .Select(f => _contentManager.Load<BitmapFont>(f))
                .ToArray();
            var textureAtlas = LoadTextureAtlas(styleSheet.TextureAtlas);
            var converterService = new GuiJsonConverterService(textureAtlas, bitmapFonts);

            serializer.Converters.Add(new GuiJsonConverter(converterService));

            styleSheet.Styles = jObject.Property(nameof(GuiStyleSheet.Styles)).Value
                .ToObject<Dictionary<string, GuiTemplate>>(serializer);

            return styleSheet;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiStyleSheet);
        }
        
        private TextureAtlas LoadTextureAtlas(string assetName)
        {
            using (var stream = TitleContainer.OpenStream(assetName))
            {
                return TextureAtlasReader.FromRawXml(_contentManager, stream);
            }
        }
    }
}