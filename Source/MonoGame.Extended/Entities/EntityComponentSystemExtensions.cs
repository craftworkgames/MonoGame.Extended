using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Entities
{
    public static class EntityComponentSystemExtensions
    {
        public static void RegisterBaseComponents(this EntityComponentSystem ecs)
        {
            ecs.RegisterComponent<SpriteComponent>();
            ecs.RegisterComponent<Transform>();
        }

        public static void RegisterBaseSystems(this EntityComponentSystem ecs)
        {
            ecs.RegisterSystem(new SpriteAnimatorSystem());
        }
    }
}
