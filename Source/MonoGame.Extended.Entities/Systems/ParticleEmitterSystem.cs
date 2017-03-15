using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Entities.Systems
{
    public class ParticleEmitterSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var components = GetComponents<TransformableComponent<ParticleEmitter>>();

            foreach (var component in components)
                component.Target.Update(deltaTime);
        }
    }
}