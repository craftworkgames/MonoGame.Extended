namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class OpacityInterpolator : Interpolator<float>
    {
        private float _delta;

        public override float EndValue
        {
            get { return _delta + StartValue; }
            set { _delta = value - StartValue; }
        }

        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Opacity = _delta*amount + StartValue;
        }
    }
}