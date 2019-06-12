using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class TextureAtlasJsonConverter : JsonConverter
    {
        private readonly ContentManager _contentManager;
        private readonly string _path;

        public TextureAtlasJsonConverter(ContentManager contentManager, string path)
        {
            _contentManager = contentManager;
            _path = path;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class InlineTextureAtlas
        {
            public string Texture { get; set; }
            public int RegionWidth { get; set; }
            public int RegionHeight { get; set; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var metadata = serializer.Deserialize<InlineTextureAtlas>(reader);

            // TODO: When we get to .NET Standard 2.1 it would be more robust to use
            // [Path.GetRelativePath](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getrelativepath?view=netstandard-2.1)
            var directory = Path.GetDirectoryName(_path);
            var assetName = Path.GetFileNameWithoutExtension(metadata.Texture);
            var relativeAssetName = Path.Combine(directory, assetName);
            var texture = _contentManager.Load<Texture2D>(relativeAssetName);

            return TextureAtlas.Create(assetName, texture, metadata.RegionWidth, metadata.RegionHeight);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureAtlas);
        }
    }
}