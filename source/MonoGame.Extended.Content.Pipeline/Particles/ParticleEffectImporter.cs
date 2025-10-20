using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content.Pipeline.Particles;

[ContentImporter(".ember", DefaultProcessor = nameof(ParticleEffectProcessor), DisplayName = "Particle Effect Importer - MonoGame.Extended")]
public class ParticleEffectImporter : ContentImporter<ContentImporterResult<ParticleEffectFileContent>>
{
    public override ContentImporterResult<ParticleEffectFileContent> Import(string filePath, ContentImporterContext context)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);

            ContentLogger.Logger = context.Logger;
            ContentLogger.Log($"Importing '{filePath}'");

            string xmlContent = File.ReadAllText(filePath);
            List<string> textureReferences = ExtractTextureReferences(filePath, context);

            ParticleEffectFileContent fileContent = new(xmlContent, textureReferences);

            ContentLogger.Log($"Imported '{filePath}'");

            return new ContentImporterResult<ParticleEffectFileContent>(filePath, fileContent);
        }
        catch (Exception e)
        {
            context.Logger.LogImportantMessage(e.StackTrace);
            throw;
        }
    }

    private List<string> ExtractTextureReferences(string filePath, ContentImporterContext context)
    {
        List<string> textureReferences = [];
        string BaseDirectory = Path.GetDirectoryName(filePath);

        using XmlReader reader = XmlReader.Create(filePath, new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true });

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "TextureRegion")
            {
                string textureName = reader.GetAttribute("Name");
                if (!string.IsNullOrEmpty(textureName))
                {
                    string texturePath = Path.Combine(BaseDirectory, textureName);

                    if (!textureReferences.Contains(texturePath))
                    {
                        textureReferences.Add(texturePath);
                        context.AddDependency(texturePath);
                        ContentLogger.Log($"Adding dependency '{texturePath}'");
                    }
                }
            }
        }

        return textureReferences;
    }
}
