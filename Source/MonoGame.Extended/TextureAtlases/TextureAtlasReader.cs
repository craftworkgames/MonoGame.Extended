using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlasReader : ContentTypeReader<TextureAtlas>
    {
        protected override TextureAtlas Read(ContentReader reader, TextureAtlas existingInstance)
        {
            var assetName = reader.GetRelativeAssetPath(reader.ReadString());
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var atlas = new TextureAtlas(texture);

            var regionCount = reader.ReadInt32();

            for (var i = 0; i < regionCount; i++)
            {
                atlas.CreateRegion(name: reader.ReadString(), x: reader.ReadInt32(), y: reader.ReadInt32(), width: reader.ReadInt32(), height: reader.ReadInt32());
            }

            return atlas;
        }
    }
}