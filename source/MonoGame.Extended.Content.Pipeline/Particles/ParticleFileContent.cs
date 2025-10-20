using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.Particles;

public record ParticleEffectFileContent(string XmlContent, List<string> TextureReferences);
