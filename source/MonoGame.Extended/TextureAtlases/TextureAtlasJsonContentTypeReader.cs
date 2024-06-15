using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization;
using ContentReaderExtensions = MonoGame.Extended.Content.ContentReaderExtensions;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlasJsonContentTypeReader : JsonContentTypeReader<Texture2DAtlas>
    {
        private static TexturePackerFile Load(ContentReader reader)
        {
            var json = reader.ReadString();
            return JsonSerializer.Deserialize<TexturePackerFile>(json);
        }

        protected override Texture2DAtlas Read(ContentReader reader, Texture2DAtlas existingInstance)
        {
            var texturePackerFile = Load(reader);
            var assetName = reader.GetRelativeAssetName(texturePackerFile.Metadata.Image);
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var atlas = new Texture2DAtlas(assetName, texture);

            var regionCount = texturePackerFile.Regions.Count;

            for (var i = 0; i < regionCount; i++)
            {
                atlas.CreateRegion(
                    ContentReaderExtensions.RemoveExtension(texturePackerFile.Regions[i].Filename),
                    texturePackerFile.Regions[i].Frame.X,
                    texturePackerFile.Regions[i].Frame.Y,
                    texturePackerFile.Regions[i].Frame.Width,
                    texturePackerFile.Regions[i].Frame.Height);
            }

            return atlas;
        }
    }
}
