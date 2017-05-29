namespace MonoGame.Extended.Particles.Modifiers
{
    public class RotationModifier : Modifier
    {
        public float RotationRate { get; set; }

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
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