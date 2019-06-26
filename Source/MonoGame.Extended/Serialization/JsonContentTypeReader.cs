using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class JsonContentTypeReader<T> : ContentTypeReader<T>
    {
        protected override T Read(ContentReader reader, T existingInstance)
        {
            var json = reader.ReadString();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }

    public class TextureAtlasJsonContentTypeReader : JsonContentTypeReader<TextureAtlas>
    {
        private TexturePackerFile Load(ContentReader reader)
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

    //public abstract class JsonContentTypeReader<TInput, TOutput> : ContentTypeReader<TOutput>
    //{
    //    protected abstract TOutput OnDeserialize(ContentReader reader, TInput texturePackerFile);

    //    protected override TOutput Read(ContentReader reader, TOutput existingInstance)
    //    {
    //        var json = reader.ReadString();

    //        using (var stringReader = new StringReader(json))
    //        using (var jsonReader = new JsonTextReader(stringReader))
    //        {
    //            var serializer = new JsonSerializer();
    //            var input = serializer.Deserialize<TInput>(jsonReader);
    //            return OnDeserialize(reader, input);
    //        }
    //    }
    //}

    //public class JsonContentTypeReader : ContentTypeReader
    //{
    //    public JsonContentTypeReader(string blah) : base(typeof(object))
    //    {
    //    }

    //    protected override void Initialize(ContentTypeReaderManager manager)
    //    {
    //        base.Initialize(manager);
    //    }

    //    protected override object Read(ContentReader input, object existingInstance)
    //    {
    //        return null;
    //    }
    //}

    //public class TexturePackerJsonContentTypeReader : JsonContentTypeReader<TexturePackerFile, TextureAtlas>
    //{
    //    protected override TextureAtlas OnDeserialize(ContentReader reader, TexturePackerFile texturePackerFile)
    //    {
    //        var assetName = reader.GetRelativeAssetName(texturePackerFile.Metadata.Image);
    //        var texture = reader.ContentManager.Load<Texture2D>(assetName);
    //        var atlas = new TextureAtlas(assetName, texture);

    //        var regionCount = texturePackerFile.Regions.Count;

    //        for (var i = 0; i < regionCount; i++)
    //        {
    //            atlas.CreateRegion(
    //                ContentReaderExtensions.RemoveExtension(texturePackerFile.Regions[i].Filename),
    //                texturePackerFile.Regions[i].Frame.X,
    //                texturePackerFile.Regions[i].Frame.Y,
    //                texturePackerFile.Regions[i].Frame.Width,
    //                texturePackerFile.Regions[i].Frame.Height);
    //        }

    //        return atlas;
    //    }
    //}
}