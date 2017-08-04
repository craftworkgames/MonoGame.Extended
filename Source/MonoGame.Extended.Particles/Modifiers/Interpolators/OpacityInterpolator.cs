namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class OpacityInterpolator : Interpolator<float>
    {
        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Opacity = (EndValue - StartValue) * amount + StartValue;
        }
    }
}