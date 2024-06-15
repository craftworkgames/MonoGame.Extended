using System.Collections.Generic;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Gui.Serialization
{
    public interface IGuiTextureRegionService : ITextureRegionService
    {
        IList<Texture2DAtlas> TextureAtlases { get; }
    }

    public class GuiTextureRegionService : TextureRegionService, IGuiTextureRegionService
    {
    }
}
