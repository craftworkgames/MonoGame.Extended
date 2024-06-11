using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Gui.Serialization;

public static class GuiJsonSerializerOptionsProvider
{
    public static JsonSerializerOptions GetOptions(ContentManager contentManager, params Type[] customControlTypes)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var textureRegionService = new GuiTextureRegionService();

        options.Converters.Add(new Vector2JsonConverter());
        options.Converters.Add(new SizeJsonConverter());
        options.Converters.Add(new Size2JsonConverter());
        options.Converters.Add(new ColorJsonConverter());
        options.Converters.Add(new ThicknessJsonConverter());
        options.Converters.Add(new ContentManagerJsonConverter<BitmapFont>(contentManager, font => font.Face));
        options.Converters.Add(new ControlStyleJsonConverter(customControlTypes));
        options.Converters.Add(new GuiTextureAtlasJsonConverter(contentManager, textureRegionService));
        options.Converters.Add(new GuiNinePatchRegion2DJsonConverter(textureRegionService));
        options.Converters.Add(new TextureRegion2DJsonConverter(textureRegionService));
        options.Converters.Add(new VerticalAlignmentConverter());
        options.Converters.Add(new HorizontalAlignmentConverter());

        return options;
    }
}
