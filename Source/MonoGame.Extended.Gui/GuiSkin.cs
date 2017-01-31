using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiSkin
    {
        public GuiSkin()
        {
            TextureAtlases = new List<TextureAtlas>();
            Fonts = new List<BitmapFont>();
            NinePatches = new List<NinePatchRegion2D>();
            Templates = new Dictionary<string, GuiControlStyle>();
        }

        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public IList<TextureAtlas> TextureAtlases { get; set; }

        [JsonProperty(Order = 2)]
        public IList<BitmapFont> Fonts { get; set; }

        [JsonProperty(Order = 3)]
        public IList<NinePatchRegion2D> NinePatches { get; set; }

        [JsonProperty(Order = 4)]
        public BitmapFont DefaultFont => Fonts.FirstOrDefault();

        [JsonProperty(Order = 5)]
        public IDictionary<string, GuiControlStyle> Templates { get; }
    }
}