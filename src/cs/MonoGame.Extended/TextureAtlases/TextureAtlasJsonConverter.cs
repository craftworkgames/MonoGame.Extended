using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;

namespace MonoGame.Extended.TextureAtlases
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
            if (reader.ValueType == typeof(string))
            {
                var textureAtlasAssetName = reader.Value.ToString();
                var contentPath = GetContentPath(textureAtlasAssetName);
                var texturePackerFile = _contentManager.Load<TexturePackerFile>(contentPath, new JsonContentLoader());
                var texture = _contentManager.Load<Texture2D>(texturePackerFile.Metadata.Image);
                //return TextureAtlas.Create(texturePackerFile.Metadata.Image, texture );
                throw new NotImplementedException();
            }
            else
            {
                var metadata = serializer.Deserialize<InlineTextureAtlas>(reader);

                // TODO: When we get to .NET Standard 2.1 it would be more robust to use
                // [Path.GetRelativePath](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getrelativepath?view=netstandard-2.1)
                var textureName = Path.GetFileNameWithoutExtension(metadata.Texture);
                var textureDirectory = Path.GetDirectoryName(metadata.Texture);
                var directory = Path.GetDirectoryName(_path);
                var relativePath = Path.Combine(_contentManager.RootDirectory, directory, textureDirectory, textureName);
                var resolvedAssetName = Path.GetFullPath(relativePath);
                Texture2D texture;
                try
                {
                    texture = _contentManager.Load<Texture2D>(resolvedAssetName);
                }
                catch (Exception ex) {
                    if (textureDirectory == null || textureDirectory == "") 
                        texture = _contentManager.Load<Texture2D>(textureName);
                    else
                        texture = _contentManager.Load<Texture2D>(textureDirectory + "/" + textureName);
                }
                return TextureAtlas.Create(resolvedAssetName, texture, metadata.RegionWidth, metadata.RegionHeight);
            }
        }

        private string GetContentPath(string relativePath)
        {
            var directory = Path.GetDirectoryName(_path);
            return Path.Combine(directory, relativePath);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureAtlas);
        }
    }
}
