using System;
using System.Diagnostics;
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
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException($"Expected {nameof(JsonTokenType.StartObject)} token");
                }

                string textureProperty = string.Empty;
                int regionWidth = 0;
                int regionHeight = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString();
                        reader.Read();

                        if (string.Equals(propertyName, "texture", StringComparison.OrdinalIgnoreCase))
                        {
                            textureProperty = reader.GetString() ?? string.Empty;
                        }
                        else if (string.Equals(propertyName, "regionWidth", StringComparison.OrdinalIgnoreCase))
                        {
                            regionWidth = reader.GetInt32();
                        }
                        else if (string.Equals(propertyName, "regionHeight", StringComparison.OrdinalIgnoreCase))
                        {
                            regionHeight = reader.GetInt32();
                        }
                        else
                        {
                            Trace.TraceWarning($"Ignoring unexpected property: {propertyName}");
                            reader.Skip();
                        }
                    }
                }

                // TODO: When we get to .NET Standard 2.1 it would be more robust to use
                // [Path.GetRelativePath](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getrelativepath?view=netstandard-2.1)
                var textureName = Path.GetFileNameWithoutExtension(textureProperty);
                var textureDirectory = Path.GetDirectoryName(textureProperty);
                var directory = Path.GetDirectoryName(_path);
                var relativePath = Path.Combine(_contentManager.RootDirectory, directory ?? string.Empty, textureDirectory ?? string.Empty, textureName);
                var resolvedAssetName = Path.GetFullPath(relativePath);
                Texture2D texture;
                try
                {
                    texture = _contentManager.Load<Texture2D>(resolvedAssetName);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(
                        $"Failed to load texture with {nameof(resolvedAssetName)}: {resolvedAssetName}. Attempting to load with {nameof(textureDirectory)}: {textureDirectory} and {nameof(textureName)}: {textureName}. Exception: {ex}");

                    if (string.IsNullOrEmpty(textureDirectory))
                    {
                        texture = _contentManager.Load<Texture2D>(textureName);
                    }
                    else
                    {
                        texture = _contentManager.Load<Texture2D>(textureDirectory + "/" + textureName);
                    }
                }
                return Texture2DAtlas.Create(resolvedAssetName, texture, regionWidth, regionHeight);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Texture2DAtlas value, JsonSerializerOptions options) { }

        private string GetContentPath(string relativePath)
        {
            var directory = Path.GetDirectoryName(_path);
            return Path.Combine(directory, relativePath);
        }
    }
}
