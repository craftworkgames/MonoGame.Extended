// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public sealed class BoundingCircle2DTest
{
    [Fact]
    public void TryGetCollision_WithOverlappingCircle_ReturnsReceiverMinimumTranslationVector()
    {
        BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 2.0f);
        BoundingCircle2D other = new BoundingCircle2D(new Vector2(3.0f, 0.0f), 2.0f);

        bool intersects = circle.TryGetCollision(other, out CollisionResult2D result);

        Assert.True(intersects);
        Assert.True(result.Intersects);
        Assert.Equal(-Vector2.UnitX, result.Normal);
        Assert.Equal(1.0f, result.PenetrationDepth);
        Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
    }

    [Fact]
    public void TryGetCollision_WithSeparatedCircle_ReturnsFalseAndNone()
    {
        BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 1.0f);
        BoundingCircle2D other = new BoundingCircle2D(new Vector2(4.0f, 0.0f), 1.0f);

        bool intersects = circle.TryGetCollision(other, out CollisionResult2D result);

        Assert.False(intersects);
        Assert.False(result.Intersects);
        Assert.Equal(CollisionResult2D.None, result);
    }

    [Fact]
    public void TryGetCollision_WithBox_ReturnsReceiverMinimumTranslationVector()
    {
        BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 2.0f);
        BoundingBox2D box = new BoundingBox2D(new Vector2(1.0f, -2.0f), new Vector2(5.0f, 2.0f));

        bool intersects = circle.TryGetCollision(box, out CollisionResult2D result);

        Assert.True(intersects);
        Assert.True(result.Intersects);
        Assert.Equal(-Vector2.UnitX, result.Normal);
        Assert.Equal(1.0f, result.PenetrationDepth);
        Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
    }

    [Fact]
    public void TryGetCollision_WithCapsule_ReturnsReceiverMinimumTranslationVector()
    {
        BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 2.0f);
        BoundingCapsule2D capsule = new BoundingCapsule2D(new Vector2(3.0f, -2.0f), new Vector2(3.0f, 2.0f), 2.0f);

        bool intersects = circle.TryGetCollision(capsule, out CollisionResult2D result);

        Assert.True(intersects);
        Assert.True(result.Intersects);
        Assert.Equal(-Vector2.UnitX, result.Normal);
        Assert.Equal(1.0f, result.PenetrationDepth);
        Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
    }

    [Fact]
    public void TryGetCollision_WithOrientedBox_ReturnsReceiverMinimumTranslationVector()
    {
        BoundingCircle2D circle = new BoundingCircle2D(Vector2.Zero, 2.0f);
        OrientedBoundingBox2D obb = new OrientedBoundingBox2D(
            new Vector2(3.0f, 0.0f),
            Vector2.UnitX,
            Vector2.UnitY,
            new Vector2(2.0f, 2.0f));

        bool intersects = circle.TryGetCollision(obb, out CollisionResult2D result);

        Assert.True(intersects);
        Assert.True(result.Intersects);
        Assert.Equal(-Vector2.UnitX, result.Normal);
        Assert.Equal(1.0f, result.PenetrationDepth);
        Assert.Equal(new Vector2(-1.0f, 0.0f), result.MinimumTranslationVector);
    }
}
