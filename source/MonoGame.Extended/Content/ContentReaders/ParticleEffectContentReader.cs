using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Content.ContentReaders;

public sealed class ParticleEffectContentReader : ContentTypeReader<ParticleEffect>
{
#if !FNA && !KNI
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
#endif

    protected override ParticleEffect Read(ContentReader input, ParticleEffect existingInstance)
    {
        string xmlContent = input.ReadString();

        byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
        using Stream stream = new MemoryStream(xmlBytes);

        // Resolve textures relative to the directory of this asset within the
        // content tree, not the content root itself.  Without this, the serializer
        // falls back to content.RootDirectory (e.g. "Content") as the base, which
        // causes content.Load to prepend that prefix a second time and produce a
        // doubled path such as "Content\Content\texture.xnb".
        string assetDirectory = Path.GetDirectoryName(input.AssetName) ?? string.Empty;
        return ParticleEffectSerializer.Deserialize(stream, input.ContentManager, assetDirectory);
    }
}
