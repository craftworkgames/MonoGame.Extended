using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using SpriteFontPlus;

namespace MonoGame.Extended.SfpGui.Serialization {
    public sealed class GuiJsonSerializer : JsonSerializer {
        public GuiJsonSerializer (ContentManager contentManager, params Type[] customControlTypes) {
            var textureRegionService = new GuiTextureRegionService ();
            Converters.Add (new Vector2JsonConverter ());
            Converters.Add (new SizeJsonConverter ());
            Converters.Add (new Size2JsonConverter ());
            Converters.Add (new ColorJsonConverter ());
            Converters.Add (new ThicknessJsonConverter ());
            //TODO: make the font nameable in SF+
            Converters.Add (new ContentManagerJsonConverter<DynamicSpriteFont> (contentManager, font => "font.Name"));
            Converters.Add (new ControlStyleJsonConverter (customControlTypes));
            Converters.Add (new GuiTextureAtlasJsonConverter (contentManager, textureRegionService));
            Converters.Add (new GuiNinePatchRegion2DJsonConverter (textureRegionService));
            Converters.Add (new TextureRegion2DJsonConverter (textureRegionService));
            Converters.Add (new AlignmentConverter ());
            ContractResolver = new ShortNameJsonContractResolver ();
            Formatting = Formatting.Indented;
        }
    }
}