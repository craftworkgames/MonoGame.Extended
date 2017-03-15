using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class DragModifier : IModifier
    {
        public float DragCoefficient { get; set; } = 0.47f;
        public float Density { get; set; } = .5f;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var drag = -DragCoefficient*Density*particle->Mass*elapsedSeconds;

                particle->Velocity = new Vector2(
                    particle->Velocity.X + particle->Velocity.X*drag,
                    particle->Velocity.Y + particle->Velocity.Y*drag);
            }
        }
    }
}