using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;

namespace MonoGame.Extended.Tests.Particles.Modifiers;

public sealed class OpacityFastFadeModifierTests
{
    [Fact]
    public unsafe void Update_ScalesOpacityByInitialOpacityAndAge()
    {
        using ParticleBuffer buffer = new ParticleBuffer(1);
        ParticleIterator releaseIterator = buffer.Release(1);
        Particle* particle = releaseIterator.Next();
        particle->InitialOpacity = 0.5f;
        particle->Age = 0.25f;
        particle->Opacity = -1f;

        OpacityFastFadeModifier modifier = new OpacityFastFadeModifier();
        modifier.Update(0f, buffer.Iterator, buffer.Count);

        Particle* result = buffer.Iterator.Next();
        Assert.Equal(0.5f * (1.0f - 0.25f), result->Opacity);
    }

    [Fact]
    public unsafe void Update_FullInitialOpacity_MatchesPreviousBehavior()
    {
        using ParticleBuffer buffer = new ParticleBuffer(1);
        ParticleIterator releaseIterator = buffer.Release(1);
        Particle* particle = releaseIterator.Next();
        particle->InitialOpacity = 1.0f;
        particle->Age = 0.4f;

        OpacityFastFadeModifier modifier = new OpacityFastFadeModifier();
        modifier.Update(0f, buffer.Iterator, buffer.Count);

        Particle* result = buffer.Iterator.Next();
        Assert.Equal(0.6f, result->Opacity);
    }
}
