namespace MonoGame.Extended.Particles.Modifiers
{
    public interface IModifier
    {
        void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);
    }
}