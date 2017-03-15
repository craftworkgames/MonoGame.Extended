namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class OpacityInterpolator : IInterpolator
    {
        private float _delta;
        public float StartValue { get; set; }

        public float EndValue
        {
            get { return _delta + StartValue; }
            set { _delta = value - StartValue; }
        }

        public unsafe void Update(float amount, Particle* particle)
        {
            particle->Opacity = _delta*amount + StartValue;
        }
    }
}