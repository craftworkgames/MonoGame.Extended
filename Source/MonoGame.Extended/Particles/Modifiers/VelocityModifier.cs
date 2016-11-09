using System;
using MonoGame.Extended.Particles.Modifiers.Interpolators;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class VelocityModifier
    {
        public IInterpolator[] Interpolators { get; set; }

        public float VelocityThreshold { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var velocityThreshold2 = VelocityThreshold*VelocityThreshold;
            var n = Interpolators.Length;

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