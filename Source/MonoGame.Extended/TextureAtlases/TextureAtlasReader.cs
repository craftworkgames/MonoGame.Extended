using System;
using System.IO;
using System.Xml;
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
                atlas.CreateRegion(
                    reader.ReadString(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32());

            return atlas;
        }

        public static TextureAtlas FromRawXml(ContentManager contentManager, Stream stream)
        {
            TextureAtlas textureAtlas = null;

            using (var xmlReader = XmlReader.Create(stream))
            {
                while (xmlReader.Read())
                {
                    var nodeType = xmlReader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        var name = xmlReader.Name;

                        if (name == "TextureAtlas")
                        {
                            var textureName = Path.GetFileNameWithoutExtension(xmlReader.GetAttribute("imagePath"));
                            var texture = contentManager.Load<Texture2D>(textureName);
                            textureAtlas = new TextureAtlas(texture);
                        }

                        if ((name == "SubTexture") && (textureAtlas != null))
                        {
                            var regionName = Path.GetFileNameWithoutExtension(xmlReader.GetAttribute("name"));
                            var x = int.Parse(xmlReader.GetAttribute("x"));
                            var y = int.Parse(xmlReader.GetAttribute("y"));
                            var width = int.Parse(xmlReader.GetAttribute("width"));
                            var height = int.Parse(xmlReader.GetAttribute("height"));
                            textureAtlas.CreateRegion(regionName, x, y, width, height);
                        }
                    }
                }
            }

            if (textureAtlas != null)
                return textureAtlas;

            throw new InvalidOperationException("Invalid Kenney XML texture atlas file");
        }
    }
}