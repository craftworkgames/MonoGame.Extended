using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Entities.Systems
{
    public class ParticleEmitterSystem : EntitySystem
    {
        public override void Update(Entity entity, GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            foreach (ParticleEmitter particleEmitter in entity.GetComponents<ParticleEmitter>())
                particleEmitter.Update(deltaTime);
        }
    }
}