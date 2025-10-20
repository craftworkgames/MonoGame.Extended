using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Content.Pipeline.Particles;

[ContentTypeWriter]
public class ParticleEffectWriter : ContentTypeWriter<ParticleEffectProcessorResult>
{
    protected override void Write(ContentWriter output, ParticleEffectProcessorResult value)
    {
        try
        {
            output.Write(value.Data.XmlContent);
        }
        catch (Exception e)
        {
            ContentLogger.Logger?.LogImportantMessage(e.StackTrace);
            throw;
        }
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(ParticleEffect).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return typeof(ContentReaders.ParticleEffectContentReader).AssemblyQualifiedName;
    }
}
