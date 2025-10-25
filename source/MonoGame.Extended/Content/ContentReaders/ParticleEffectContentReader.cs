using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Content.ContentReaders;

public sealed class ParticleEffectContentReader : ContentTypeReader<ParticleEffect>
{
    protected override ParticleEffect Read(ContentReader input, ParticleEffect existingInstance)
    {
        string xmlContent = input.ReadString();

        byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlContent);
        using Stream stream = new MemoryStream(xmlBytes);
        return ParticleEffectSerializer.Deserialize(stream, input.ContentManager);
    }
}
