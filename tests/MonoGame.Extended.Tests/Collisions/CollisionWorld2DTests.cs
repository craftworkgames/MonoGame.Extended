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
    public void QueryCandidates_WhenLayerNameIsNull_UsesDefaultLayerBroadphase()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));

        world.Insert(actor);

        ICollisionActor candidate = world.QueryCandidates(actor.Shape.BoundingBox).Single();
        Assert.Same(actor, candidate);
    }

    [Fact]
    public void QueryCandidates_WhenNamedLayerIsProvided_UsesMatchingLayerBroadphase()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        NamedLayerActor actor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(actor, "actors");

        ICollisionActor candidate = world.QueryCandidates(actor.Shape.BoundingBox, "actors").Single();
        Assert.Same(actor, candidate);
    }

    [Fact]
    public void QueryCandidates_WhenCrossLayerCollisionIsDisabled_ReturnsNoCandidates()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f))));

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        ICollisionActor[] candidates = world.QueryCandidates(defaultActor, "actors").ToArray();

        Assert.Empty(candidates);
    }

    [Fact]
    public void QueryCollisions_WhenCrossLayerCollisionIsDisabled_DoesNotTouchOtherActorShape()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        CountingShapeActor defaultActor = new CountingShapeActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f))));
        CountingShapeActor namedActor = new CountingShapeActor(
            2,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f))));

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");
        defaultActor.ResetShapeAccessCount();
        namedActor.ResetShapeAccessCount();

        CollisionEvent2D[] collisions = world.QueryCollisions(defaultActor, "actors").ToArray();

        Assert.Empty(collisions);
        Assert.Equal(0, defaultActor.ShapeAccessCount);
        Assert.Equal(0, namedActor.ShapeAccessCount);
    }

    [Fact]
    public void QueryCandidates_WhenCrossLayerCollisionIsEnabled_ReturnsCandidates()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f))));

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.EnableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        ICollisionActor candidate = world.QueryCandidates(defaultActor, "actors").Single();

        Assert.Same(namedActor, candidate);
    }

    [Fact]
    public void QueryCollisionPairs_WhenCrossLayerCollisionIsDisabled_DoesNotTouchActorShapes()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        CountingShapeActor defaultActor = new CountingShapeActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f))));
        CountingShapeActor namedActor = new CountingShapeActor(
            2,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f))));

        world.AddLayer("actors", namedLayer);
        world.DisableCollisionBetweenLayers(CollisionWorld2D.DefaultLayerName, "actors");
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");
        defaultActor.ResetShapeAccessCount();
        namedActor.ResetShapeAccessCount();

        CollisionPair2D[] pairs = world.QueryCollisionPairs(CollisionWorld2D.DefaultLayerName, "actors").ToArray();

        Assert.Empty(pairs);
        Assert.Equal(0, defaultActor.ShapeAccessCount);
        Assert.Equal(0, namedActor.ShapeAccessCount);
    }

    [Fact]
    public void QueryCollisions_WhenShapesOverlap_ReturnsCollisionResult()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            1,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1f, 0f), new Vector2(2f, 2f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        CollisionEvent2D collision = world.QueryCollisions(defaultActor, "actors").Single();

        Assert.Same(namedActor, collision.Other);
        Assert.True(collision.Result.Intersects);
    }

    [Fact]
    public void QueryCollisions_WhenCandidateDoesNotProduceCollisionResult_SkipsCandidate()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            1,
            new CollisionShape2D(new BoundingCapsule2D(new Vector2(1f, -1f), new Vector2(1f, 1f), 0.5f)));

        world.AddLayer("actors", namedLayer);
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        CollisionEvent2D[] collisions = world.QueryCollisions(defaultActor, "actors").ToArray();

        Assert.Empty(collisions);
    }

    [Fact]
    public void QueryCollisions_WhenCircleAndBoxShapesOverlap_ReturnsCollisionResult()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        NamedLayerActor circleActor = new NamedLayerActor(
            1,
            new CollisionShape2D(new BoundingCircle2D(Vector2.Zero, 2.0f)));
        NamedLayerActor boxActor = new NamedLayerActor(
            2,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1.0f, -2.0f), new Vector2(4.0f, 4.0f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(circleActor);
        world.Insert(boxActor, "actors");

        CollisionEvent2D collision = world.QueryCollisions(circleActor, "actors").Single();

        Assert.Same(boxActor, collision.Other);
        Assert.True(collision.Result.Intersects);
        Assert.NotEqual(Vector2.Zero, collision.Result.MinimumTranslationVector);
    }

    [Fact]
    public void QueryCollisions_WhenObbAndBoxShapesOverlap_ReturnsCollisionResult()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        NamedLayerActor obbActor = new NamedLayerActor(
            1,
            new CollisionShape2D(OrientedBoundingBox2D.CreateFromRotation(
                new Vector2(3.0f, 2.0f),
                MathHelper.PiOver4,
                new Vector2(2.0f, 2.0f))));
        NamedLayerActor boxActor = new NamedLayerActor(
            2,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(4.0f, 4.0f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(obbActor);
        world.Insert(boxActor, "actors");

        CollisionEvent2D collision = world.QueryCollisions(obbActor, "actors").Single();

        Assert.Same(boxActor, collision.Other);
        Assert.True(collision.Result.Intersects);
        Assert.NotEqual(Vector2.Zero, collision.Result.MinimumTranslationVector);
    }

    [Fact]
    public void QueryCollisionPairs_WhenWorldContainsMixedShapes_ReturnsOnlySupportedPairs()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        NamedLayerActor circleActor = new NamedLayerActor(
            1,
            new CollisionShape2D(new BoundingCircle2D(Vector2.Zero, 2.0f)));
        NamedLayerActor polygonActor = new NamedLayerActor(
            2,
            new CollisionShape2D(new BoundingPolygon2D(
                new[]
                {
                    new Vector2(1.0f, -1.0f),
                    new Vector2(3.0f, -1.0f),
                    new Vector2(3.0f, 1.0f),
                    new Vector2(1.0f, 1.0f)
                },
                new[]
                {
                    -Vector2.UnitY,
                    Vector2.UnitX,
                    Vector2.UnitY,
                    -Vector2.UnitX
                })));
        NamedLayerActor unsupportedCapsuleActor = new NamedLayerActor(
            3,
            new CollisionShape2D(new BoundingCapsule2D(new Vector2(0.5f, -1.0f), new Vector2(0.5f, 1.0f), 0.5f)));

        world.Insert(circleActor);
        world.Insert(polygonActor);
        world.Insert(unsupportedCapsuleActor);

        CollisionPair2D[] pairs = world.QueryCollisionPairs(
            CollisionWorld2D.DefaultLayerName,
            CollisionWorld2D.DefaultLayerName).ToArray();

        Assert.Single(pairs);
        Assert.Equal(circleActor.Id, pairs[0].FirstId);
        Assert.Equal(unsupportedCapsuleActor.Id, pairs[0].SecondId);
        Assert.True(pairs[0].FirstResult.Intersects);
    }

    [Fact]
    public void QueryCollisionPairs_WhenCollisionDataIsNeeded_ReturnsPairWithResults()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            7,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1f, 0f), new Vector2(2f, 2f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        CollisionPair2D pair = world.QueryCollisionPairs(CollisionWorld2D.DefaultLayerName, "actors").Single();

        Assert.Same(defaultActor, pair.First);
        Assert.Same(namedActor, pair.Second);
        Assert.True(pair.FirstResult.Intersects);
        Assert.Equal(defaultActor.Id, pair.FirstId);
        Assert.Equal(namedActor.Id, pair.SecondId);
    }

    [Fact]
    public void QueryCollisionPairs_WhenSecondResultIsRequested_ReturnsOppositeResultDirection()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        Layer namedLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor defaultActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f)));
        NamedLayerActor namedActor = new NamedLayerActor(
            7,
            new CollisionShape2D(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1f, 0f), new Vector2(2f, 2f))));

        world.AddLayer("actors", namedLayer);
        world.Insert(defaultActor);
        world.Insert(namedActor, "actors");

        CollisionPair2D pair = world.QueryCollisionPairs(CollisionWorld2D.DefaultLayerName, "actors").Single();

        Assert.Equal(-pair.FirstResult.Normal, pair.SecondResult.Normal);
        Assert.Equal(pair.FirstResult.PenetrationDepth, pair.SecondResult.PenetrationDepth);
        Assert.Equal(-pair.FirstResult.MinimumTranslationVector, pair.SecondResult.MinimumTranslationVector);
    }

    [Fact]
    public void QueryCollisionPairs_WhenActorsShareLayer_ReturnsPairOnlyOnce()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(64, 64)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor firstActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(2f, 2f)));
        BasicActor secondActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1f, 0f), new Vector2(2f, 2f)));

        world.Insert(firstActor);
        world.Insert(secondActor);

        CollisionPair2D[] pairs = world.QueryCollisionPairs(CollisionWorld2D.DefaultLayerName, CollisionWorld2D.DefaultLayerName).ToArray();

        Assert.Single(pairs);
        Assert.Equal(firstActor.Id, pairs[0].FirstId);
        Assert.Equal(secondActor.Id, pairs[0].SecondId);
    }

    [Fact]
    public void QueryCollisionPairs_WhenActorsSpanMultipleBroadphaseCells_SuppressesDuplicates()
    {
        Layer defaultLayer = new Layer(new SpatialHash(new SizeF(32, 32)));
        CollisionWorld2D world = new CollisionWorld2D(defaultLayer);
        BasicActor firstActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(16f, 16f), new Vector2(48f, 48f)));
        BasicActor secondActor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(32f, 32f), new Vector2(32f, 32f)));

        world.Insert(firstActor);
        world.Insert(secondActor);

        CollisionPair2D[] pairs = world.QueryCollisionPairs(CollisionWorld2D.DefaultLayerName, CollisionWorld2D.DefaultLayerName).ToArray();

        Assert.Single(pairs);
        Assert.Equal(firstActor.Id, pairs[0].FirstId);
        Assert.Equal(secondActor.Id, pairs[0].SecondId);
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

    private sealed class CountingShapeActor : ICollisionActor
    {
        private readonly CollisionShape2D _shape;

        public CountingShapeActor(int id, CollisionShape2D shape)
        {
            Id = id;
            _shape = shape;
        }

        public int Id { get; }

        public int ShapeAccessCount { get; private set; }

        public CollisionShape2D Shape
        {
            get
            {
                ShapeAccessCount++;
                return _shape;
            }
        }

        public void ResetShapeAccessCount()
        {
            ShapeAccessCount = 0;
        }
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
