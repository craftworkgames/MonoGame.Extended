using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers
{
    public unsafe class VortexModifier : Modifier
    {
        // Note: not the real-life one
        private const float _gravConst = 100000f;

        public Vector2 Position { get; set; }
        public float Mass { get; set; }
        public float MaxSpeed { get; set; }

        public override void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var diff = Position + particle->TriggerPos - particle->Position;

                var distance2 = diff.LengthSquared();

                var speedGain = _gravConst*Mass/distance2;
                speedGain = Math.Max(Math.Min(speedGain, MaxSpeed), -MaxSpeed) * elapsedSeconds;
                // normalize distances and multiply by speedGain
                diff.Normalize();
                particle->Velocity += diff*speedGain;
            }
        }
    }
}
