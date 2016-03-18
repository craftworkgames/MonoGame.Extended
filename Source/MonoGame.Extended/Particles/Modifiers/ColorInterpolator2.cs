namespace MonoGame.Extended.Particles.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the color of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColorInterpolator2 : IModifier
    {
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public HslColor InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public HslColor FinalColour { get; set; }

        public unsafe void Update(float elapsedseconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = new HslColor(FinalColour.H - InitialColour.H,
                                   FinalColour.S - InitialColour.S,
                                   FinalColour.L - InitialColour.L);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Colour = new HslColor(
                    InitialColour.H + delta.H * particle->Age,
                    InitialColour.S + delta.S * particle->Age,
                    InitialColour.L + delta.L * particle->Age);
            }
        }
    }
}