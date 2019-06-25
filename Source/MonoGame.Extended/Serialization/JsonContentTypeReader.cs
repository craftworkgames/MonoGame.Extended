using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class JsonContentTypeReader : ContentTypeReader
    {
        //protected override object Read(ContentReader input, object existingInstance)
        //{
        //    var json = input.ReadString();
        //    return null;
        //}

        public JsonContentTypeReader() : base(typeof(TextureAtlas))
        {
        }

        protected override object Read(ContentReader reader, object existingInstance)
        {
            var json = reader.ReadString();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var serializer = new JsonSerializer();
                var texturePackerFile = serializer.Deserialize<TexturePackerFile>(jsonReader);


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




                return texturePackerFile;
            }
        }
    }
}