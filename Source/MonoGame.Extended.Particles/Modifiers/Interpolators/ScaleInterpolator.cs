using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class ScaleInterpolator : IInterpolator
    {
        private Vector2 _delta;
        public Vector2 StartValue { get; set; }

        public Vector2 EndValue
        {
            get { return _delta + StartValue; }
            set { _delta = value - StartValue; }
        }

        public unsafe void Update(float amount, Particle* particle)
        {
            particle->Scale = _delta*amount + StartValue;
        }
    }
}