namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public abstract class Interpolator
    {
        protected Interpolator()
        {
            Name = GetType().Name;
        }

        public string Name { get; set; }
        public abstract unsafe void Update(float amount, Particle* particle);
    }

    public abstract class Interpolator<T> : Interpolator
    {
        /// <summary>
        /// Gets or sets the intial value when the particles are created.
        /// </summary>
        public virtual T StartValue { get; set; }

        /// <summary>
        /// Gets or sets the final value when the particles are retired.
        /// </summary>
        public virtual T EndValue { get; set; }
    }
}