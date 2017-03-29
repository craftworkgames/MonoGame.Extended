using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public sealed class GuiJsonSerializer : JsonSerializer
    {
        public GuiJsonSerializer(ContentManager contentManager)
        {
            var textureRegionService = new GuiTextureRegionService();
            Converters.Add(new Vector2JsonConverter());
            Converters.Add(new Size2JsonConverter());
            Converters.Add(new ColorJsonConverter());
            Converters.Add(new ThicknessJsonConverter());
            Converters.Add(new ContentManagerJsonConverter<BitmapFont>(contentManager, font => font.Name));
            Converters.Add(new GuiControlStyleJsonConverter());
            Converters.Add(new GuiTextureAtlasJsonConverter(contentManager, textureRegionService));
            Converters.Add(new GuiNinePatchRegion2DJsonConverter(textureRegionService));
            Converters.Add(new TextureRegion2DJsonConverter(textureRegionService));
            ContractResolver = new GuiJsonContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}