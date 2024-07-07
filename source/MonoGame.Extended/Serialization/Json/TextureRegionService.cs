using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Serialization.Json
{
    public interface ITextureRegionService
    {
        Texture2DRegion GetTextureRegion(string name);
    }

    public class TextureRegionService : ITextureRegionService
    {
        public TextureRegionService()
        {
            TextureAtlases = new List<Texture2DAtlas>();
        }

        public IList<Texture2DAtlas> TextureAtlases { get; }

        public Texture2DRegion GetTextureRegion(string name)
        {
            return TextureAtlases
                .Select(textureAtlas => textureAtlas.GetRegion(name))
                .FirstOrDefault(region => region != null);
        }
    }
}
