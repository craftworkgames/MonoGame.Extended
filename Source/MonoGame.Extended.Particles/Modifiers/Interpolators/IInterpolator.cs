namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public interface IInterpolator
    {
        unsafe void Update(float amount, Particle* particle);
    }

    public interface IInterpolator<T> : IInterpolator
    {
        T StartValue { get; set; }
        T EndValue { get; set; }
    }
}