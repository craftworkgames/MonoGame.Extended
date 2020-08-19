using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using ContentReaderExtensions = MonoGame.Extended.Content.ContentReaderExtensions;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlasJsonContentTypeReader : JsonContentTypeReader<TextureAtlas>
    {
        private static TexturePackerFile Load(ContentReader reader)
        {
            var json = reader.ReadString();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<TexturePackerFile>(jsonReader);
            }
        }

        protected override TextureAtlas Read(ContentReader reader, TextureAtlas existingInstance)
        {
            var texturePackerFile = Load(reader);
            var assetName = reader.GetRelativeAssetName(texturePackerFile.Metadata.Image);
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var atlas = new TextureAtlas(assetName, texture);

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