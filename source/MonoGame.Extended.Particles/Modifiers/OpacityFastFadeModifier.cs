namespace MonoGame.Extended.Particles.Modifiers
{
    public sealed class OpacityFastFadeModifier : Modifier
    {
        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Opacity = 1.0f - particle->Age;
            }
        }
    }
}