namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    /// <summary>
    ///     Defines a modifier which interpolates the color of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColorInterpolator : IInterpolator
    {
        /// <summary>
        ///     Gets or sets the initial color of particles when they are released.
        /// </summary>
        public HslColor InitialColor { get; set; }

        /// <summary>
        ///     Gets or sets the final color of particles when they are retired.
        /// </summary>
        public HslColor FinalColor { get; set; }

        public unsafe void Update(float amount, Particle* particle)
        {
            particle->Color = HslColor.Lerp(InitialColor, FinalColor, amount);
        }
    }
}