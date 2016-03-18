namespace MonoGame.Extended.Particles.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the color of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColorInterpolator2 : IModifier
    {
        /// <summary>
        /// Gets or sets the initial color of particles when they are released.
        /// </summary>
        public HslColor InitialColor { get; set; }

        /// <summary>
        /// Gets or sets the final color of particles when they are retired.
        /// </summary>
        public HslColor FinalColor { get; set; }

        public unsafe void Update(float elapsedseconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = new HslColor(FinalColor.H - InitialColor.H,
                                   FinalColor.S - InitialColor.S,
                                   FinalColor.L - InitialColor.L);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Color = new HslColor(
                    InitialColor.H + delta.H * particle->Age,
                    InitialColor.S + delta.S * particle->Age,
                    InitialColor.L + delta.L * particle->Age);
            }
        }
    }
}