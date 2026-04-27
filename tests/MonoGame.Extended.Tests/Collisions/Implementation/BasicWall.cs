namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicWall : BasicActor
    {
        public BasicWall()
        {
        }

        public BasicWall(BoundingBox2D bounds)
            : base(bounds)
        {
        }

        public BasicWall(BoundingCircle2D bounds)
            : base(bounds)
        {
        }

        public BasicWall(OrientedBoundingBox2D bounds)
            : base(bounds)
        {
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }
}
