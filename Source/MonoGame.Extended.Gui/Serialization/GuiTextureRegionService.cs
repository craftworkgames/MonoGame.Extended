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

    public class GuiTextureRegionService : TextureRegionService, IGuiTextureRegionService
    {
    }
}