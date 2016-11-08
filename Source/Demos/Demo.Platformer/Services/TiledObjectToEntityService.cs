using System;
using System.Collections.Generic;
using Demo.Platformer.Entities;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Maps.Tiled;

namespace Demo.Platformer.Services
{
    public class TiledObjectToEntityService
    {
        private readonly EntityFactory _entityFactory;

        public TiledObjectToEntityService(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
            _createEntityFunctions = new Dictionary<string, Func<TiledObject, Entity>>
            {
                {"Spawn", tiledObject => _entityFactory.CreatePlayer(tiledObject.Position)},
                {"Solid", tiledObject => _entityFactory.CreateSolid(tiledObject.Position, tiledObject.Size) },
                {"Deadly", tiledObject => _entityFactory.CreateDeadly(tiledObject.Position, tiledObject.Size) },
                {"BadGuy", tiledObject => _entityFactory.CreateBadGuy(tiledObject.Position, tiledObject.Size) }
            };
        }

        private readonly Dictionary<string, Func<TiledObject, Entity>> _createEntityFunctions;

        public void CreateEntities(TiledObject[] tiledObjects)
        {
            foreach (var tiledObject in tiledObjects)
                _createEntityFunctions[tiledObject.Type](tiledObject);
        }
    }
}
