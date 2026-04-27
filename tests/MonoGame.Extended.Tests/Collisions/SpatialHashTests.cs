using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests;

public class SpatialHashTests
{
    private readonly BoundingBox2D _box = BoundingBox2D.CreateFromPositionAndSize(new Vector2(10f, 10f), new Vector2(20f, 20f));
    private readonly BoundingBox2D _queryBounds = new BoundingBox2D(new Vector2(10, 10), new Vector2(30, 30));
    private readonly BoundingBox2D _movedQueryBounds = new BoundingBox2D(new Vector2(130, 130), new Vector2(150, 150));

    private SpatialHash CreateSpatialHash()
    {
        return new SpatialHash(new SizeF(64, 64));
    }

    [Fact]
    public void CollisionOneTrueTest()
    {
        SpatialHash hash = CreateSpatialHash();
        hash.Insert(new BasicActor(_box));
        IEnumerable<ICollisionActor> collisions = hash.Query(_queryBounds);
        Assert.Equal(1, collisions.Count());
    }

    [Fact]
    public void CollisionTwoTest()
    {
        SpatialHash hash = CreateSpatialHash();
        hash.Insert(new BasicActor(_box));
        hash.Insert(new BasicActor(_box));
        IEnumerable<ICollisionActor> collisions = hash.Query(_queryBounds);
        Assert.Equal(2, collisions.Count());
    }

    [Fact]
    public void QueryWhenActorOverlapsMultipleCellsReturnsUniqueActor()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(32f, 32f), new Vector2(96f, 96f)));

        hash.Insert(actor);

        List<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(32f, 32f), new Vector2(128f, 128f))).ToList();

        Assert.Single(collisions);
        Assert.Same(actor, collisions[0]);
    }

    [Fact]
    public void RemoveAfterInsertThenQueryReturnsNoActors()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(_box);

        hash.Insert(actor);
        bool removed = hash.Remove(actor);

        IEnumerable<ICollisionActor> collisions = hash.Query(_queryBounds);

        Assert.True(removed);
        Assert.Empty(collisions);
    }

    [Fact]
    public void ResetAfterActorMovesThenQueryUsesUpdatedBounds()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(_box);

        hash.Insert(actor);
        actor.SetBounds(BoundingBox2D.CreateFromPositionAndSize(new Vector2(130f, 130f), new Vector2(20f, 20f)));

        hash.Reset();

        IEnumerable<ICollisionActor> oldAreaCollisions = hash.Query(_queryBounds);
        IEnumerable<ICollisionActor> movedAreaCollisions = hash.Query(_movedQueryBounds);

        Assert.Empty(oldAreaCollisions);
        Assert.Single(movedAreaCollisions);
    }

    [Fact]
    public void QueryWithCircleActorUsesBroadphaseBoundingBox()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(new BoundingCircle2D(new Vector2(100f, 100f), 20f));

        hash.Insert(actor);

        IEnumerable<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(80f, 80f), new Vector2(120f, 120f)));

        Assert.Single(collisions);
        Assert.Same(actor, collisions.Single());
    }

    [Fact]
    public void QueryWithOrientedRectangleActorCanReturnBroadphaseFalsePositive()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(OrientedBoundingBox2D.CreateFromRotation(
            new Vector2(128f, 128f),
            MathHelper.PiOver4,
            new Vector2(48f, 48f)));

        hash.Insert(actor);

        IEnumerable<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(94f, 94f), new Vector2(100f, 100f)));

        Assert.Single(collisions);
        Assert.Same(actor, collisions.Single());
    }
}
