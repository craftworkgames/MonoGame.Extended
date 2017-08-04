using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class ScaleInterpolator : Interpolator<Vector2>
    {
        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Scale = (EndValue - StartValue) * amount + StartValue;
        }
    }
}