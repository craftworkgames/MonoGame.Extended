using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class LinearGravityModifier : Modifier
    {
        public Vector2 Direction { get; set; }
        public float Strength { get; set; }

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var vector = Direction*(Strength*elapsedSeconds);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Velocity = new Vector2(
                    particle->Velocity.X + vector.X*particle->Mass,
                    particle->Velocity.Y + vector.Y*particle->Mass);
            }
        }
    }
}