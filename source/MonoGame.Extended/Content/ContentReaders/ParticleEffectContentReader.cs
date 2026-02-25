using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Content.ContentReaders;

public sealed class ParticleEffectContentReader : ContentTypeReader<ParticleEffect>
{
    /// <summary>
    /// Registers this <see cref="ContentTypeReader"/> with the <see cref="ContentTypeReaderManager"/>
    /// so it is resolved without reflection.
    /// </summary>
    /// <remarks>
    /// Call this method once during application startup when publishing with
    /// <c>PublishAot</c> or <c>PublishTrimmed</c>.
    /// </remarks>
    public static void Register() =>
        ContentTypeReaderManager.AddTypeCreator(
            typeof(ParticleEffectContentReader).AssemblyQualifiedName,
            () => new ParticleEffectContentReader());

    protected override ParticleEffect Read(ContentReader input, ParticleEffect existingInstance)
    {
        string xmlContent = input.ReadString();

        byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
        using Stream stream = new MemoryStream(xmlBytes);
        return ParticleEffectSerializer.Deserialize(stream, input.ContentManager);
    }
}
