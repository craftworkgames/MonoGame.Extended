using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests;

public class CollisionResult2DTest
{
    [Fact]
    public void Constructor_WithValues_SetsFields()
    {
        Vector2 normal = new Vector2(0.0f, -1.0f);
        Vector2 minimumTranslationVector = new Vector2(0.0f, -5.0f);

        CollisionResult2D result = new CollisionResult2D(true, normal, 5.0f, minimumTranslationVector);

        Assert.True(result.Intersects);
        Assert.Equal(normal, result.Normal);
        Assert.Equal(5.0f, result.PenetrationDepth);
        Assert.Equal(minimumTranslationVector, result.MinimumTranslationVector);
    }

    [Fact]
    public void None_ReturnsDefaultNonIntersectingResult()
    {
        CollisionResult2D result = CollisionResult2D.None;

        Assert.False(result.Intersects);
        Assert.Equal(Vector2.Zero, result.Normal);
        Assert.Equal(0.0f, result.PenetrationDepth);
        Assert.Equal(Vector2.Zero, result.MinimumTranslationVector);
    }

    [Fact]
    public void Default_ReturnsNonIntersectingResult()
    {
        CollisionResult2D result = default;

        Assert.False(result.Intersects);
        Assert.Equal(Vector2.Zero, result.Normal);
        Assert.Equal(0.0f, result.PenetrationDepth);
        Assert.Equal(Vector2.Zero, result.MinimumTranslationVector);
    }

    [Fact]
    public void MinimumTranslationVector_EqualsNormalTimesPenetrationDepth()
    {
        Vector2 normal = new Vector2(1.0f, 0.0f);
        float penetrationDepth = 3.0f;
        Vector2 minimumTranslationVector = normal * penetrationDepth;

        CollisionResult2D result = new CollisionResult2D(true, normal, penetrationDepth, minimumTranslationVector);

        Assert.Equal(result.Normal * result.PenetrationDepth, result.MinimumTranslationVector);
    }
}
