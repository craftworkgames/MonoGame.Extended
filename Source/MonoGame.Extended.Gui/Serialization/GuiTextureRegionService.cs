using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Serialization
{
    public interface IGuiTextureRegionService : ITextureRegionService
    {
        IList<TextureAtlas> TextureAtlases { get; }
        IList<NinePatchRegion2D> NinePatches { get; }
    }

    public class GuiTextureRegionService : IGuiTextureRegionService
    {
        public GuiTextureRegionService()
        {
            TextureAtlases = new List<TextureAtlas>();
            NinePatches = new List<NinePatchRegion2D>();
        }

        public IList<TextureAtlas> TextureAtlases { get; }
        public IList<NinePatchRegion2D> NinePatches { get; }

        public TextureRegion2D GetTextureRegion(string name)
        {
            var ninePatch = NinePatches.FirstOrDefault(p => p.Name == name);

            if (ninePatch != null)
                return ninePatch;

            return TextureAtlases
                .Select(textureAtlas => textureAtlas.GetRegion(name))
                .FirstOrDefault(region => region != null);
        }
    }
}