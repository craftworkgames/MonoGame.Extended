using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content.ContentReaders
{
    public class Texture2DAtlasReader : ContentTypeReader<Texture2DAtlas>
    {
        protected override Texture2DAtlas Read(ContentReader reader, Texture2DAtlas existingInstance)
        {
            var assetName = reader.GetRelativeAssetName(reader.ReadString());
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var atlas = new Texture2DAtlas(assetName, texture);

            var regionCount = reader.ReadInt32();

            for (var i = 0; i < regionCount; i++)
            {
                atlas.CreateRegion(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadString());
            }

            return atlas;
        }
    }
}
