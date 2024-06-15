using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Serialization
{
    public interface ITextureRegionService
    {
        TextureRegion GetTextureRegion(string name);
    }

    public class TextureRegionService : ITextureRegionService
    {
        public TextureRegionService()
        {
            TextureAtlases = new List<TextureAtlas>();
        }

        public IList<TextureAtlas> TextureAtlases { get; }

        public TextureRegion GetTextureRegion(string name)
        {
            return TextureAtlases
                .Select(textureAtlas => textureAtlas.GetRegion(name))
                .FirstOrDefault(region => region != null);
        }
    }
}
