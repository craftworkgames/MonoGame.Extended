using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.QuadTree;

using CollisionQuadTree = MonoGame.Extended.Collisions.QuadTree.QuadTree;

namespace MonoGame.Extended.Collisions.Tests;

public class QuadTreeTests
{
    private readonly BoundingBox2D _quadTreeArea = new BoundingBox2D(new Vector2(-10f, -15f), new Vector2(10f, 15f));

    private CollisionQuadTree CreateTree()
    {
        BoundingBox2D bounds = _quadTreeArea;
        CollisionQuadTree tree = new CollisionQuadTree(bounds);
        return tree;
    }

    [Fact]
    public void Constructor_WhenCreatedWithBounds_StoresBoundsAndStartsAsLeaf()
    {
        BoundingBox2D bounds = new BoundingBox2D(new Vector2(-10f, -15f), new Vector2(10f, 15f));
        CollisionQuadTree tree = new CollisionQuadTree(bounds);

        Assert.Equal(bounds, tree.NodeBounds);
        Assert.True(tree.IsLeaf);
    }

    [Fact]
    public void NumTargets_WhenTreeIsEmpty_ReturnsZero()
    {
        CollisionQuadTree tree = CreateTree();

        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void NumTargets_WhenTreeContainsOneActor_ReturnsOne()
    {
        CollisionQuadTree tree = CreateTree();
        BasicActor actor = new BasicActor();

        tree.Insert(new QuadtreeData(actor));

        Assert.Equal(1, tree.NumTargets());
    }

    [Fact]
    public void NumTargets_WhenTreeContainsMultipleActors_ReturnsCount()
    {
        CollisionQuadTree tree = CreateTree();

        for (int i = 0; i < 5; i++)
        {
            tree.Insert(new QuadtreeData(new BasicActor()));
        }

        Assert.Equal(5, tree.NumTargets());
    }

    [Fact]
    public void NumTargets_WhenActorsAreInsertedIncrementally_ReturnsRunningCount()
    {
        CollisionQuadTree tree = CreateTree();

        for (int i = 0; i < 1000; i++)
        {
            tree.Insert(new QuadtreeData(new BasicActor()));
            Assert.Equal(i + 1, tree.NumTargets());
        }

        Assert.Equal(1000, tree.NumTargets());
    }

    [Fact]
    public void Insert_WhenOneActorIsInserted_IncreasesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        BasicActor actor = new BasicActor();

        tree.Insert(new QuadtreeData(actor));

        Assert.Equal(1, tree.NumTargets());
    }

    [Fact]
    public void Insert_WhenOneActorOverlapsQuadrants_CountsActorOnce()
    {
        CollisionQuadTree tree = CreateTree();
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(-2.5f, -2.5f), new Vector2(5f, 5f)));

        tree.Insert(new QuadtreeData(actor));

        Assert.Equal(1, tree.NumTargets());
    }

    [Fact]
    public void Insert_WhenMultipleActorsAreInserted_IncreasesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();

        for (int i = 0; i < 10; i++)
        {
            tree.Insert(new QuadtreeData(new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, Vector2.One))));
        }

        Assert.Equal(10, tree.NumTargets());
    }

    [Fact]
    public void Insert_WhenManyActorsAreInserted_IncreasesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();

        for (int i = 0; i < 1000; i++)
        {
            tree.Insert(new QuadtreeData(new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, Vector2.One))));
        }

        Assert.Equal(1000, tree.NumTargets());
    }

    [Fact]
    public void Insert_WhenMultipleActorsOverlapQuadrants_CountsActorsOnceEach()
    {
        CollisionQuadTree tree = CreateTree();

        for (int i = 0; i < 10; i++)
        {
            BasicActor actor = new BasicActor(new BoundingBox2D(new Vector2(-10f, -15f), new Vector2(10f, 15f)));
            tree.Insert(new QuadtreeData(actor));
        }

        Assert.Equal(10, tree.NumTargets());
    }

    [Fact]
    public void Remove_WhenOnlyActorIsRemoved_LeavesTreeEmpty()
    {
        BasicActor actor = new BasicActor(BoundingBox2D.CreateFromPositionAndSize(new Vector2(-5f, -7f), new Vector2(10f, 15f)));
        QuadtreeData data = new QuadtreeData(actor);
        CollisionQuadTree tree = CreateTree();

        tree.Insert(data);
        tree.Remove(data);

        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void Remove_WhenTwoActorsAreRemoved_UpdatesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        List<QuadtreeData> inserted = new List<QuadtreeData>();
        int numTargets = 2;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, Vector2.One)));
            tree.Insert(data);
            inserted.Add(data);
        }

        int inTree = numTargets;
        Assert.Equal(inTree, tree.NumTargets());

        foreach (QuadtreeData data in inserted)
        {
            tree.Remove(data);
            Assert.Equal(--inTree, tree.NumTargets());
        }
    }

    [Fact]
    public void Remove_WhenThreeActorsAreRemoved_UpdatesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        List<QuadtreeData> inserted = new List<QuadtreeData>();
        int numTargets = 3;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, Vector2.One)));
            tree.Insert(data);
            inserted.Add(data);
        }

        int inTree = numTargets;
        Assert.Equal(inTree, tree.NumTargets());

        foreach (QuadtreeData data in inserted)
        {
            tree.Remove(data);
            Assert.Equal(--inTree, tree.NumTargets());
        }
    }

    [Fact]
    public void RemoveFromAllParents_WhenManyActorsAreRemoved_UpdatesTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        List<QuadtreeData> inserted = new List<QuadtreeData>();
        int numTargets = 1000;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, Vector2.One)));
            tree.Insert(data);
            inserted.Add(data);
        }

        int inTree = numTargets;
        Assert.Equal(inTree, tree.NumTargets());

        foreach (QuadtreeData data in inserted)
        {
            data.RemoveFromAllParents();
            Assert.Equal(--inTree, tree.NumTargets());
        }
    }

    [Fact]
    public void Shake_WhenTreeIsEmpty_KeepsTargetCountAtZero()
    {
        CollisionQuadTree tree = CreateTree();

        tree.Shake();

        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenSplitTreeIsEmpty_KeepsTargetCountAtZero()
    {
        CollisionQuadTree tree = CreateTree();

        tree.Split();
        tree.Shake();

        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenSplitTreeContainsActor_KeepsTargetCount()
    {
        CollisionQuadTree tree = CreateTree();

        tree.Split();
        tree.Insert(new QuadtreeData(new BasicActor()));
        tree.Shake();

        Assert.Equal(1, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenTreeContainsOneActor_KeepsTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = 1;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        tree.Shake();

        Assert.Equal(numTargets, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenTreeContainsTwoActors_KeepsTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = 2;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        tree.Shake();

        Assert.Equal(numTargets, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenTreeContainsThreeActors_KeepsTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = 3;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        tree.Shake();

        Assert.Equal(numTargets, tree.NumTargets());
    }

    [Fact]
    public void Shake_WhenTreeContainsManyActors_KeepsTargetCount()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = CollisionQuadTree.DefaultMaxObjectsPerNode + 1;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        tree.Shake();

        Assert.Equal(numTargets, tree.NumTargets());
    }

    [Fact]
    public void Query_WhenTreeIsEmpty_ReturnsNoResults()
    {
        CollisionQuadTree tree = CreateTree();

        List<QuadtreeData> query = tree.Query(_quadTreeArea);

        Assert.Empty(query);
        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void Query_WhenAreaDoesNotOverlapTree_ReturnsNoResults()
    {
        CollisionQuadTree tree = CreateTree();
        BoundingBox2D area = new BoundingBox2D(new Vector2(100f, 100f), new Vector2(101f, 101f));

        List<QuadtreeData> query = tree.Query(area);

        Assert.Empty(query);
        Assert.Equal(0, tree.NumTargets());
    }

    [Fact]
    public void Query_WhenLeafNodeContainsActor_ReturnsActor()
    {
        CollisionQuadTree tree = CreateTree();
        BasicActor actor = new BasicActor();

        tree.Insert(new QuadtreeData(actor));

        List<QuadtreeData> query = tree.Query(_quadTreeArea);

        Assert.Single(query);
        Assert.Equal(tree.NumTargets(), query.Count);
    }

    [Fact]
    public void Query_WhenLeafNodeDoesNotOverlapArea_ReturnsNoResults()
    {
        CollisionQuadTree tree = CreateTree();
        BasicActor actor = new BasicActor();
        BoundingBox2D area = new BoundingBox2D(new Vector2(100f, 100f), new Vector2(101f, 101f));

        tree.Insert(new QuadtreeData(actor));

        List<QuadtreeData> query = tree.Query(area);

        Assert.Empty(query);
    }

    [Fact]
    public void Query_WhenLeafNodeContainsMultipleActors_ReturnsAllActors()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = CollisionQuadTree.DefaultMaxObjectsPerNode;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        List<QuadtreeData> query = tree.Query(_quadTreeArea);

        Assert.Equal(numTargets, query.Count);
        Assert.Equal(tree.NumTargets(), query.Count);
    }

    [Fact]
    public void Query_WhenNonLeafTreeContainsManyActors_ReturnsAllActors()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = 2 * CollisionQuadTree.DefaultMaxObjectsPerNode;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        List<QuadtreeData> query = tree.Query(_quadTreeArea);

        Assert.Equal(numTargets, query.Count);
        Assert.Equal(tree.NumTargets(), query.Count);
    }

    [Fact]
    public void Query_WhenCalledTwiceConsecutively_ReturnsSameResultCount()
    {
        CollisionQuadTree tree = CreateTree();
        int numTargets = 2 * CollisionQuadTree.DefaultMaxObjectsPerNode;

        for (int i = 0; i < numTargets; i++)
        {
            QuadtreeData data = new QuadtreeData(new BasicActor());
            tree.Insert(data);
        }

        List<QuadtreeData> query1 = tree.Query(_quadTreeArea);
        List<QuadtreeData> query2 = tree.Query(_quadTreeArea);

        Assert.Equal(numTargets, query1.Count);
        Assert.Equal(tree.NumTargets(), query1.Count);
        Assert.Equal(query1.Count, query2.Count);
    }
}
