using System.Collections.Generic;
using System.ComponentModel;
using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class AgeModifier : Modifier
    {
        [EditorBrowsable(EditorBrowsableState.Always)]
        public List<Interpolator> Interpolators { get; set; } = new List<Interpolator>();

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var n = Interpolators.Count;
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