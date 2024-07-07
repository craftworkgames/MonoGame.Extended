using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS.Tests;

public class WorldManagerTests
{
    private GameTime _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

    [Fact]
    public void CrudEntity()
    {
        var dummySystem = new DummySystem();
        var worldBuilder = new WorldBuilder();
        worldBuilder.AddSystem(dummySystem);
        var world = worldBuilder.Build();

        world.Initialize();

        var entity = world.CreateEntity();
        entity.Attach(new Transform2());
        world.Update(_gameTime);
        world.Draw(_gameTime);
        var otherEntity = world.GetEntity(entity.Id);
        Assert.Equal(entity, otherEntity);
        Assert.True(otherEntity.Has<Transform2>());
        Assert.Contains(entity.Id, dummySystem.AddedEntitiesId);

        entity.Destroy();
        world.Update(_gameTime);
        world.Draw(_gameTime);
        otherEntity = world.GetEntity(entity.Id);
        Assert.Null(otherEntity);
        Assert.Contains(entity.Id, dummySystem.RemovedEntitiesId);
    }

    private class DummyComponent { }

    private class DummySystem : EntitySystem, IUpdateSystem, IDrawSystem
    {
        public List<int> AddedEntitiesId { get; } = new();
        public List<int> RemovedEntitiesId { get; } = new();

        public DummySystem() : base(Aspect.All(typeof(DummyComponent))) { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            // Do NOT initialize mapper in  order to test: https://github.com/craftworkgames/MonoGame.Extended/issues/707
        }

        public void Draw(GameTime gameTime) { }

        public void Update(GameTime gameTime) { }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);
            AddedEntitiesId.Add(entityId);
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);
            RemovedEntitiesId.Add(entityId);
        }
    }
}
