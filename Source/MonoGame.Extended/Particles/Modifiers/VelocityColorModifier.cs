using System;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class VelocityColorModifier : IModifier
    {
        public HslColor StationaryColour { get; set; }
        public HslColor VelocityColour { get; set; }
        public float VelocityThreshold { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var velocity2 = particle->Velocity.X * particle->Velocity.X +
                                particle->Velocity.Y * particle->Velocity.Y;
                var deltaColour = VelocityColour - StationaryColour;

                if (velocity2 >= velocityThreshold2)
                {
                    VelocityColour.CopyTo(out particle->Colour);
                }
                else
                {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;

                    particle->Colour = new HslColor(
                        deltaColour.H * t + StationaryColour.H,
                        deltaColour.S * t + StationaryColour.S,
                        deltaColour.L * t + StationaryColour.L);
                }
            }
        }
    }
}