using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests;

public class SpatialHashTests
{
    private SpatialHash generateSpatialHash() => new SpatialHash(new SizeF(64, 64));
    private readonly RectangleF RECT = new RectangleF(10, 10, 20, 20);

    [Fact]
    public void CollisionOneTrueTest()
    {
        var hash = generateSpatialHash();
        hash.Insert(new BasicActor()
        {
            Bounds = RECT,
        });
        var collisions = hash.Query(RECT);
        Assert.Equal(1, collisions.Count());
    }

    [Fact]
    public void CollisionTwoTest()
    {
        var hash = generateSpatialHash();
        hash.Insert(new BasicActor
        {
            Bounds = RECT,
        });
        hash.Insert(new BasicActor
        {
            Bounds = RECT,
        });
        var collisions = hash.Query(RECT);
        Assert.Equal(2, collisions.Count());
    }
}
