using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Entities.Systems
{
    public class ParticleEmitterSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var emitters = GetComponents<ParticleEmitter>();

            foreach (var particleEmitter in emitters)
                particleEmitter.Update(deltaTime);
        }
    }
}