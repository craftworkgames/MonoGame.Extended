using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class VelocityHueModifier : IModifier
    {
        public float StationaryHue { get; set; }
        public float VelocityHue { get; set; }
        public float VelocityThreshold { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var velocity2 = particle->Velocity.LengthSquared();

                float h;
                if (velocity2 >= velocityThreshold2)
                {
                    h = VelocityHue;
                }
                else
                {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;
                    h = MathHelper.Lerp(StationaryHue, VelocityHue, t);
                }
                particle->Color = new HslColor(h, particle->Color.S, particle->Color.L);
            }
        }
    }
}