using Demo.Platformer.Entities.Components;
using Demo.Platformer.Entities.Factories;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Demo.Platformer.Entities
{
    public static class Entities
    {
        public const string Player = "Spawn";
        public const string BadGuy = "BadGuy";
        public const string Solid = "Solid";
        public const string Deadly = "Deadly";

        public static IReadOnlyDictionary<string, ITiledEntityFactory> Factories
            => new ReadOnlyDictionary<string, ITiledEntityFactory>(_factories);

        #region Entity (Factory) Defintions

        private static readonly Dictionary<string, ICollection<Type>> _definitions
            = new Dictionary<string, ICollection<Type>>()
            {
                [Player] = new Type[]
                {
                    typeof(CollisionBody),
                    typeof(SpriteComponent),
                    typeof(CharacterState),
                    typeof(Player),
                },

                [BadGuy] = new Type[]
                {
                    typeof(CollisionBody),
                    typeof(SpriteComponent),
                    typeof(CharacterState),
                    typeof(Enemy),
                },

                [Deadly] = new Type[]
                {
                    typeof(CollisionBody),
                    typeof(Deadly),
                },

                [Solid] = new Type[]
                {
                    typeof(CollisionBody),
                }
            }; 

        private static readonly Dictionary<string, ITiledEntityFactory> _factories
            = new Dictionary<string, ITiledEntityFactory>()
            {
                [Player] = new PlayerEntityFactory(),
                [BadGuy] = new BadGuyFactory(),
                [Deadly] = new StaticEntityFactory(),
                [Solid] = new StaticEntityFactory()
            };

        #endregion

        public static void RegisterEntities(EntityComponentSystem ecs, TiledEntityFactoryCollection factories)
        {
            foreach (var def in _definitions)
                ecs.RegisterEntity(def.Key, def.Value);

            foreach (var def in _factories)
                factories.Add(def.Key, def.Value);
        }
    }
}
