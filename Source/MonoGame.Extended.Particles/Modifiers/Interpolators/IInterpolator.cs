namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public interface IInterpolator
    {
        unsafe void Update(float amount, Particle* particle);
    }
}