using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Serialization
{
    public interface ITextureRegionService
    {
        TextureRegion2D GetTextureRegion(string name);
    }
}