using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class ScaleInterpolator : Interpolator<Vector2>
    {
        private Vector2 _delta;

        public override Vector2 EndValue
        {
            get { return _delta + StartValue; }
            set { _delta = value - StartValue; }
        }

        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Scale = _delta*amount + StartValue;
        }
    }
}