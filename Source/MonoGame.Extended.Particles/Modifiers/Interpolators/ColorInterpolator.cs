using System;

namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    /// <summary>
    ///     Defines a modifier which interpolates the color of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColorInterpolator : IInterpolator<HslColor>
    {
        /// <summary>
        ///     Gets or sets the initial color of particles when they are released.
        /// </summary>
        public HslColor StartValue { get; set; }

        /// <summary>
        ///     Gets or sets the final color of particles when they are retired.
        /// </summary>
        public HslColor EndValue { get; set; }

        [Obsolete("Use StartValue instead")]
        public HslColor InitialColor
        {
            get { return StartValue; }
            set { StartValue = value; }
        }

        [Obsolete("Use EndValue instead")]
        public HslColor FinalColor
        {
            get { return EndValue; }
            set { EndValue = value; }
        }

        public unsafe void Update(float amount, Particle* particle)
        {
            particle->Color = HslColor.Lerp(StartValue, EndValue, amount);
        }

    }
}