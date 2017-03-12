using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class AgeModifier : IModifier
    {
        public IInterpolator[] Interpolators { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var n = Interpolators.Length;
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                for (var i = 0; i < n; i++)
                {
                    var interpolator = Interpolators[i];
                    interpolator.Update(particle->Age, particle);
                }
            }
        }
    }
}