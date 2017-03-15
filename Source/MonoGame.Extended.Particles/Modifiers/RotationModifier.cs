namespace MonoGame.Extended.Particles.Modifiers
{
    public class RotationModifier : IModifier
    {
        public float RotationRate { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var rotationRateDelta = RotationRate*elapsedSeconds;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Rotation += rotationRateDelta;
            }
        }
    }
}