using MonoGame.Extended;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class HueInterpolator : Interpolator<float>
    {
        private float _delta;

        public override float EndValue
        {
            get { return _delta + StartValue; }
            set { _delta = value - StartValue; }
        }

        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Color = new HslColor(
                amount*_delta + StartValue,
                particle->Color.S,
                particle->Color.L);
        }
    }
}