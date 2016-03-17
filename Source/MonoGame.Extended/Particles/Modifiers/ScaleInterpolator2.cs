namespace MonoGame.Extended.Particles.Modifiers {
    public class ScaleInterpolator2 : IModifier {
        public Vector InitialScale { get; set; }
        public Vector FinalScale { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var delta = FinalScale - InitialScale;

            while (iterator.HasNext) {
                var particle = iterator.Next();
                particle->Scale = delta * particle->Age + InitialScale;
            }
        }
    }
}