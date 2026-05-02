using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests;

public class BasicActor : ICollisionActor
{
    private static int s_nextId = 1;

    public int Id { get; } = s_nextId++;

    public Vector2 Position { get; private set; }

    public CollisionShape2D Shape { get; private set; }

    public BasicActor()
    {
        SetBounds(BoundingBox2D.CreateFromPositionAndSize(Vector2.Zero, new Vector2(1f, 1f)));
    }

    public BasicActor(BoundingBox2D bounds)
    {
        SetBounds(bounds);
    }

    public BasicActor(BoundingCircle2D bounds)
    {
        SetBounds(bounds);
    }

    public BasicActor(OrientedBoundingBox2D bounds)
    {
        SetBounds(bounds);
    }

    public void SetBounds(BoundingBox2D bounds)
    {
        Position = bounds.Min;
        Shape = new CollisionShape2D(bounds);
    }

    public void SetBounds(BoundingCircle2D bounds)
    {
        Position = bounds.Center;
        Shape = new CollisionShape2D(bounds);
    }

    public void SetBounds(OrientedBoundingBox2D bounds)
    {
        Position = bounds.Center;
        Shape = new CollisionShape2D(bounds);
    }
}
