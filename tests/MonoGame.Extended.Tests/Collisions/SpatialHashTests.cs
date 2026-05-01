using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests;

public class SpatialHashTests
{
    private readonly BoundingBox2D _box = BoundingBox2D.CreateFromPositionAndSize(new Vector2(10f, 10f), new Vector2(20f, 20f));
    private readonly BoundingBox2D _queryBounds = new BoundingBox2D(new Vector2(10f, 10f), new Vector2(30f, 30f));
    private readonly BoundingBox2D _movedQueryBounds = new BoundingBox2D(new Vector2(130f, 130f), new Vector2(150f, 150f));

    private SpatialHash CreateSpatialHash()
    {
        return new SpatialHash(new SizeF(64f, 64f));
    }

    [Fact]
    public void Query_WhenOneActorOverlapsQueryBounds_ReturnsOneActor()
    {
        SpatialHash hash = CreateSpatialHash();

        hash.Insert(new BasicActor(_box));

        IEnumerable<ICollisionActor> collisions = hash.Query(_queryBounds);

        Assert.Equal(1, collisions.Count());
    }

    [Fact]
    public void Query_WhenTwoActorsOverlapQueryBounds_ReturnsTwoActors()
    {
        SpatialHash hash = CreateSpatialHash();

        hash.Insert(new BasicActor(_box));
        hash.Insert(new BasicActor(_box));

        IEnumerable<ICollisionActor> collisions = hash.Query(_queryBounds);

        Assert.Equal(2, collisions.Count());
    }

    [Fact]
    public void Query_WhenActorOverlapsMultipleCells_ReturnsUniqueActor()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(32f, 32f), new Vector2(96f, 96f)));

        hash.Insert(actor);

        List<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(32f, 32f), new Vector2(128f, 128f))).ToList();

        Assert.Single(collisions);
        Assert.Same(actor, collisions[0]);
    }

    [Fact]
    public void Query_WhenBoundsSpanCellBoundary_ReturnsActorsFromAllCoveredCells()
    {
        SpatialHash hash = new SpatialHash(new SizeF(10f, 10f));
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(19f, 0f), new Vector2(1f, 1f)));

        hash.Insert(actor);

        List<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(9f, 0f), new Vector2(21f, 1f))).ToList();

        Assert.Single(collisions);
        Assert.Same(actor, collisions[0]);
    }

    [Fact]
    public void Query_WhenBoundsUseNegativeCoordinates_ReturnsActorFromNegativeCell()
    {
        SpatialHash hash = new SpatialHash(new SizeF(10f, 10f));
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(-9f, -9f), new Vector2(4f, 4f)));

        hash.Insert(actor);

        List<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(-10f, -10f), new Vector2(-1f, -1f))).ToList();

        Assert.Single(collisions);
        Assert.Same(actor, collisions[0]);
    }

    [Fact]
    public void Insert_WhenActorAlreadyExists_DoesNotDuplicateStoredActor()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(_box);

        hash.Insert(actor);
        hash.Insert(actor);

        List<ICollisionActor> collisions = hash.Query(_queryBounds).ToList();
        List<ICollisionActor> storedActors = new List<ICollisionActor>();

        foreach (ICollisionActor storedActor in hash)
        {
            storedActors.Add(storedActor);
        }

        Assert.Single(collisions);
        Assert.Single(storedActors);
        Assert.Same(actor, collisions[0]);
        Assert.Same(actor, storedActors[0]);
    }

    [Fact]
    public void Remove_WhenActorWasInsertedAndThenRemoved_ReturnsNoActorsFromQuery()
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
    public void Reset_WhenActorMovesBeforeReset_UsesUpdatedBoundsForQuery()
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
    public void Query_WhenActorUsesCircleBounds_UsesBroadphaseBoundingBox()
    {
        SpatialHash hash = CreateSpatialHash();
        BasicActor actor = new BasicActor(new BoundingCircle2D(new Vector2(100f, 100f), 20f));

        hash.Insert(actor);

        IEnumerable<ICollisionActor> collisions = hash.Query(new BoundingBox2D(new Vector2(80f, 80f), new Vector2(120f, 120f)));

        Assert.Single(collisions);
        Assert.Same(actor, collisions.Single());
    }

    [Fact]
    public void Query_WhenActorUsesOrientedRectangleBounds_CanReturnBroadphaseFalsePositive()
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
