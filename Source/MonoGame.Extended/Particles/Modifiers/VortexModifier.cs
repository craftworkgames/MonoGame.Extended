namespace MonoGame.Extended.Particles.Modifiers
{
    public unsafe class VortexModifier : IModifier
    {
        public Vector Position { get; set; }
        public float Mass { get; set; }
        public float MaxSpeed { get; set; }
        // Note: not the real-life one
        private const float GravConst = 100000f;

        public void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var diff = Position + particle->TriggerPos - particle->Position;

                var distance2 = diff.LengthSq;

                var speedGain = GravConst * Mass / distance2 * elapsedSeconds;
                // normalize distances and multiply by speedGain
                particle->Velocity += diff.Axis * speedGain;
            }
        }
    }
}