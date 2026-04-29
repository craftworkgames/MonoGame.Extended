using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Layers;

namespace MonoGame.Extended.Collisions.Tests;

public class CollisionWorld2DTests
{
    [Fact]
    public void Insert_WhenActorIsInsertedWithoutLayerName_StoresActorInDefaultLayer()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.Insert(actor);

        ICollisionActor storedActor = defaultLayer.Space.Query(actor.Shape.BoundingBox).Single();
        Assert.Same(actor, storedActor);
    }

    [Fact]
    public void Insert_WhenLayerNameIsProvided_StoresActorInMatchingLayer()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        NamedLayerActor actor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(actor, "actors");

        ICollisionActor storedActor = namedLayer.Space.Query(actor.Shape.BoundingBox).Single();
        Assert.Same(actor, storedActor);
    }

    [Fact]
    public void Insert_WhenTargetLayerIsMissing_ThrowsUndefinedLayerException()
    {
        CollisionWorld2D world = new CollisionWorld2D();
        NamedLayerActor actor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f))));

        void Act()
        {
            world.Insert(actor, "actors");
        }

        Assert.Throws<UndefinedLayerException>(Act);
    }

    [Fact]
    public void Insert_WhenActorAlreadyExistsInWorldOnDifferentLayer_ThrowsInvalidOperationException()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.AddLayer("actors", namedLayer);
        world.Insert(actor);

        void Act()
        {
            world.Insert(actor, "actors");
        }

        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Insert_WhenSameActorIsInsertedIntoDifferentWorlds_AllowsBothInsertions()
    {
        Layer firstDefaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer secondDefaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D firstWorld = new CollisionWorld2D(firstDefaultLayer);
        CollisionWorld2D secondWorld = new CollisionWorld2D(secondDefaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        firstWorld.Insert(actor);
        secondWorld.Insert(actor);

        Assert.Same(actor, firstDefaultLayer.Space.Query(actor.Shape.BoundingBox).Single());
        Assert.Same(actor, secondDefaultLayer.Space.Query(actor.Shape.BoundingBox).Single());
    }

    [Fact]
    public void Contains_WhenActorIsPresent_ReturnsTrue()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.Insert(actor);

        Assert.True(world.Contains(actor));
    }

    [Fact]
    public void Contains_WhenActorIsNotPresent_ReturnsFalse()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        Assert.False(world.Contains(actor));
    }

    [Fact]
    public void TryGetLayerName_WhenActorIsPresent_ReturnsTrueAndAssignedLayer()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.AddLayer("actors", namedLayer);
        world.Insert(actor, "actors");

        bool found = world.TryGetLayerName(actor, out string layerName);

        Assert.True(found);
        Assert.Equal("actors", layerName);
    }

    [Fact]
    public void TryGetLayerName_WhenActorIsNotPresent_ReturnsFalse()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        bool found = world.TryGetLayerName(actor, out string layerName);

        Assert.False(found);
        Assert.Null(layerName);
    }

    [Fact]
    public void GetLayerName_WhenActorIsPresent_ReturnsAssignedLayer()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.Insert(actor);

        Assert.Equal(CollisionWorld2D.DefaultLayerName, world.GetLayerName(actor));
    }

    [Fact]
    public void GetLayerName_WhenActorIsNotPresent_ThrowsInvalidOperationException()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        void Act()
        {
            world.GetLayerName(actor);
        }

        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void MoveToLayer_WhenTargetLayerExists_MovesActorAndUpdatesMembership()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.AddLayer("actors", namedLayer);
        world.Insert(actor);

        world.MoveToLayer(actor, "actors");

        Assert.Equal("actors", world.GetLayerName(actor));
        Assert.Empty(defaultLayer.Space.Query(actor.Shape.BoundingBox));
        Assert.Same(actor, namedLayer.Space.Query(actor.Shape.BoundingBox).Single());
    }

    [Fact]
    public void MoveToLayer_WhenActorIsNotPresent_ThrowsInvalidOperationException()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.AddLayer("actors", namedLayer);

        void Act()
        {
            world.MoveToLayer(actor, "actors");
        }

        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void MoveToLayer_WhenTargetLayerIsMissing_ThrowsUndefinedLayerException()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.Insert(actor);

        void Act()
        {
            world.MoveToLayer(actor, "actors");
        }

        Assert.Throws<UndefinedLayerException>(Act);
    }

    [Fact]
    public void RebuildDynamicLayers_WhenLayersAreRegistered_ResetsAllLayers()
    {
        ResetTrackingLayer defaultLayer = new ResetTrackingLayer(new SpatialHash(new SizeF(64, 64)));
        ResetTrackingLayer namedLayer = new ResetTrackingLayer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);

        world.AddLayer("actors", namedLayer);

        world.RebuildDynamicLayers();

        Assert.Equal(1, defaultLayer.ResetCallCount);
        Assert.Equal(1, namedLayer.ResetCallCount);
    }

    [Fact]
    public void RebuildDynamicLayers_WhenLayerIsStatic_StillCallsReset()
    {
        ResetTrackingLayer defaultLayer = new ResetTrackingLayer(new SpatialHash(new SizeF(64, 64)))
        {
            IsDynamic = false
        };
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);

        world.RebuildDynamicLayers();

        Assert.Equal(1, defaultLayer.ResetCallCount);
    }

    [Fact]
    public void AddLayer_WhenDefaultLayerExists_EnablesCollisionWithDefaultLayer()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);

        world.AddLayer("actors", namedLayer);

        Assert.True(world.IsCollisionEnabledBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors"));
    }

    [Fact]
    public void AddLayer_WhenNamedLayerIsRegistered_EnablesSelfCollision()
    {
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D();

        world.AddLayer("actors", namedLayer);

        Assert.True(world.IsCollisionEnabledBetweenLayers("actors", "actors"));
    }

    [Fact]
    public void DisableCollisionBetweenLayers_WhenCalled_DisablesExistingRule()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");

        Assert.False(world.IsCollisionEnabledBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors"));
    }

    [Fact]
    public void EnableCollisionBetweenLayers_WhenCalledAfterDisable_ReenablesExistingRule()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.EnableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");

        Assert.True(world.IsCollisionEnabledBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors"));
    }

    private sealed class NamedLayerActor : ICollisionActor
    {
        public NamedLayerActor(int id, CollisionShape2D shape)
        {
            Id = id;
            Shape = shape;
        }

        public int Id { get; }

        public CollisionShape2D Shape { get; }
    }

    private sealed class ResetTrackingLayer : Layer
    {
        public ResetTrackingLayer(ICollisionBroadphase2D spaceAlgorithm)
            : base(spaceAlgorithm)
        {
        }

        public int ResetCallCount { get; private set; }

        public override void Reset()
        {
            ResetCallCount++;
            base.Reset();
        }
    }
}
