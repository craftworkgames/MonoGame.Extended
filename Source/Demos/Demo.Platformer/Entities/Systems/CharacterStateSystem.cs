using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    public sealed class CharacterStateSystem : EntitySystem
    {
        protected override void ComponentAdded(Entity entity, Type type, object component)
        {
            if (type == typeof(CharacterState))
                ((CharacterState)component).OnKilled += () =>
                {
                    entity.Destroy();
                };
        }
    }
}