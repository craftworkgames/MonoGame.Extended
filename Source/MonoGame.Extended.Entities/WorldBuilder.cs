﻿using System.Collections.Generic;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class WorldBuilder
    {
        private readonly List<ISystem> _systems = new List<ISystem>();

        public WorldBuilder AddSystem(ISystem system)
        {
            _systems.Add(system);
            return this;
        }

        public World Build()
        {
            var world = new World();

            foreach (var system in _systems)
                world.RegisterSystem(system);

            return world;
        }
    }
}