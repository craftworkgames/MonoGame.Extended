using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Collisions.Tests;

public class QuadTreeSpaceTests
{
    private readonly BoundingBox2D _boundary = new BoundingBox2D(new Vector2(0f, 0f), new Vector2(256f, 256f));
    private readonly BoundingBox2D _box = BoundingBox2D.CreateFromPositionAndSize(new Vector2(10f, 10f), new Vector2(20f, 20f));
    private readonly BoundingBox2D _queryBounds = new BoundingBox2D(new Vector2(10f, 10f), new Vector2(30f, 30f));
    private readonly BoundingBox2D _movedQueryBounds = new BoundingBox2D(new Vector2(130f, 130f), new Vector2(150f, 150f));

    private QuadTreeSpace CreateQuadTreeSpace()
    {
        return new QuadTreeSpace(_boundary);
    }

    [Fact]
    public void Insert_WhenActorIsInsertedAndQueried_ReturnsActor()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(_box);

        space.Insert(actor);

        IEnumerable<ICollisionActor> collisions = space.Query(_queryBounds);

        Assert.Single(collisions);
    }

    [Fact]
    public void Remove_WhenActorWasInsertedAndThenRemoved_ReturnsNoActorsFromQuery()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(_box);

        space.Insert(actor);
        bool removed = space.Remove(actor);

        IEnumerable<ICollisionActor> collisions = space.Query(_queryBounds);

        Assert.True(removed);
        Assert.Empty(collisions);
    }

    [Fact]
    public void Reset_WhenActorMovesBeforeReset_UsesUpdatedBoundsForQuery()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(_box);

        space.Insert(actor);
        actor.SetBounds(BoundingBox2D.CreateFromPositionAndSize(new Vector2(130f, 130f), new Vector2(20f, 20f)));

        space.Reset();

        IEnumerable<ICollisionActor> oldAreaCollisions = space.Query(_queryBounds);
        IEnumerable<ICollisionActor> movedAreaCollisions = space.Query(_movedQueryBounds);

        Assert.Empty(oldAreaCollisions);
        Assert.Single(movedAreaCollisions);
    }

    [Fact]
    public void Query_WhenActorOverlapsMultipleQuadrants_ReturnsUniqueActor()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(120f, 120f), new Vector2(32f, 32f)));

        space.Insert(actor);

        List<ICollisionActor> collisions = space.Query(new BoundingBox2D(new Vector2(120f, 120f), new Vector2(152f, 152f))).ToList();

        Assert.Single(collisions);
        Assert.Same(actor, collisions[0]);
    }

    [Fact]
    public void Query_WhenActorUsesCircleBounds_UsesBroadphaseBoundingBox()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(new BoundingCircle2D(new Vector2(100f, 100f), 20f));

        space.Insert(actor);

        IEnumerable<ICollisionActor> collisions = space.Query(new BoundingBox2D(new Vector2(80f, 80f), new Vector2(120f, 120f)));

        Assert.Single(collisions);
        Assert.Same(actor, collisions.Single());
    }

    [Fact]
    public void Query_WhenActorUsesOrientedRectangleBounds_CanReturnBroadphaseFalsePositive()
    {
        QuadTreeSpace space = CreateQuadTreeSpace();
        BasicActor actor = new BasicActor(OrientedBoundingBox2D.CreateFromRotation(
            new Vector2(128f, 128f),
            MathHelper.PiOver4,
            new Vector2(48f, 48f)));

        space.Insert(actor);

        IEnumerable<ICollisionActor> collisions = space.Query(new BoundingBox2D(new Vector2(94f, 94f), new Vector2(100f, 100f)));

        Assert.Single(collisions);
        Assert.Same(actor, collisions.Single());
    }
}
