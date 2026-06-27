using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MonoGame.Extended.Content.Pipeline.Particles;

[ContentProcessor(DisplayName = "Particle Effect Processor - MonoGame.Extended")]
public class ParticleEffectProcessor : ContentProcessor<ContentImporterResult<ParticleEffectFileContent>, ParticleEffectProcessorResult>
{
    public override ParticleEffectProcessorResult Process(ContentImporterResult<ParticleEffectFileContent> input, ContentProcessorContext context)
    {
        try
        {
            ContentLogger.Logger = context.Logger;
            ContentLogger.Log("Processing particle effect");

            ParticleEffectFileContent fileContent = input.Data;

            foreach (string texturePath in fileContent.TextureReferences)
            {
                ContentLogger.Log($"Validating texture '{texturePath}'");
#if KNI || FNA
                // KNI and FNA do not use the new external ref calls from MonoGame's new
                // content builder project
                ExternalReference<Texture2DContent> externalRef = new ExternalReference<Texture2DContent>(texturePath);
                context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(externalRef, nameof(TextureProcessor));
#else
                ExternalReference<TextureContent> externalRef = new ExternalReference<TextureContent>(texturePath);
                context.BuildAndLoadAsset<TextureContent, TextureContent>(
                    externalRef,
                    new TextureImporter(),
                    new TextureProcessor());
#endif                    
            }

            ContentLogger.Log("Processed particle effect");

            return new ParticleEffectProcessorResult(fileContent);
        }
        catch (Exception e)
        {
            context.Logger.LogImportantMessage(e.Message);
            throw;
        }
    }
}
