using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionActor : IActor
    {
        public CollisionActor(IActorTarget target, int width, int height)
        {
            Width = width;
            Height = height;
            _target = target;
        }

        private readonly IActorTarget _target;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Position
        {
            get { return _target.Position; }
            set { _target.Position = value; }
        }

        public RectangleF GetAxisAlignedBoundingBox()
        {
            return new RectangleF(Position.X, Position.Y, Width, Height);
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            _target.OnCollision(collisionInfo);
        }
    }
}