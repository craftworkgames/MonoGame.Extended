namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    /// <summary>
    /// Defines a modifier which interpolates the color of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColorInterpolator : Interpolator<HslColor>
    {
        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Color = HslColor.Lerp(StartValue, EndValue, amount);
        }
    }
}