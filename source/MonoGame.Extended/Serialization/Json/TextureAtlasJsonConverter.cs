using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Content.TexturePacker;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Serialization.Json
{
    public class TextureAtlasJsonConverter : JsonConverter<Texture2DAtlas>
    {
        private readonly ContentManager _contentManager;
        private readonly string _path;

        public TextureAtlasJsonConverter(ContentManager contentManager, string path)
        {
            _contentManager = contentManager;
            _path = path;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Texture2DAtlas);

        public override Texture2DAtlas Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // TODO: (Aristurtle 05/20/2024) What is this for? It's just an if block that throws an exception. Need
                // to investigate.
                var textureAtlasAssetName = reader.GetString();
                var contentPath = GetContentPath(textureAtlasAssetName);
                var texturePackerFile = _contentManager.Load<TexturePackerFileContent>(contentPath, new JsonContentLoader());
                var texture = _contentManager.Load<Texture2D>(texturePackerFile.Meta.Image);
                //return TextureAtlas.Create(texturePackerFile.Metadata.Image, texture );
                throw new NotImplementedException();
            }
            else
            {
                var metadata = JsonSerializer.Deserialize<InlineTextureAtlas>(ref reader, options);

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
                catch (Exception ex)
                {
                    if (textureDirectory == null || textureDirectory == "")
                        texture = _contentManager.Load<Texture2D>(textureName);
                    else
                        texture = _contentManager.Load<Texture2D>(textureDirectory + "/" + textureName);
                }
                return Texture2DAtlas.Create(resolvedAssetName, texture, metadata.RegionWidth, metadata.RegionHeight);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Texture2DAtlas value, JsonSerializerOptions options) { }


        // ReSharper disable once ClassNeverInstantiated.Local
        private class InlineTextureAtlas
        {
            public string Texture { get; set; }
            public int RegionWidth { get; set; }
            public int RegionHeight { get; set; }
        }

        private string GetContentPath(string relativePath)
        {
            var directory = Path.GetDirectoryName(_path);
            return Path.Combine(directory, relativePath);
        }
    }
}
