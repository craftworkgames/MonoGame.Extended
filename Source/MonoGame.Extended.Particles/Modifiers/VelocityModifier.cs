using System;
using System.Collections.Generic;
using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class VelocityModifier : Modifier
    {
        public List<Interpolator> Interpolators { get; set; } = new List<Interpolator>();

        public float VelocityThreshold { get; set; }

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var velocityThreshold2 = VelocityThreshold*VelocityThreshold;
            var n = Interpolators.Count;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var velocity2 = particle->Velocity.LengthSquared();

                if (velocity2 >= velocityThreshold2)
                {
                    for (var i = 0; i < n; i++)
                    {
                        var interpolator = Interpolators[i];
                        interpolator.Update(1, particle);
                    }
                }
                else
                {
                    var t = (float) Math.Sqrt(velocity2)/VelocityThreshold;
                    for (var i = 0; i < n; i++)
                    {
                        var interpolator = Interpolators[i];
                        interpolator.Update(t, particle);
                    }
                }
            }
        }
    }
}