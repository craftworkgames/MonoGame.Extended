using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace MonoGame.Extended.Benchmarks.Collisions.Utils;

public class Collider: ICollisionActor
{
    public Collider(Point2 position)
    {
        Bounds = new RectangleF(position, new Size2(1, 1));
    }

    public Collider(IShapeF shape)
    {
        Bounds = shape;
    }

    public IShapeF Bounds { get; set; }
    public Vector2 Shift { get; set; }

    public Point2 Position {
        get => Bounds.Position;
        set => Bounds.Position = value;
    }

    public void OnCollision(CollisionEventArgs collisionInfo)
    {
    }
}
