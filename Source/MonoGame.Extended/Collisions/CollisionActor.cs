using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionActor : IActorTarget
    {
        private readonly IActorTarget _target;

        public RectangleF BoundingBox => _target.BoundingBox;

        public Vector2 Position
        {
            get { return _target.Position; }
            set { _target.Position = value; }
        }

        public Vector2 Velocity
        {
            get { return _target.Velocity; }
            set { _target.Velocity = value; }
        }

        public CollisionActor(IActorTarget target)
        {
            _target = target;
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            _target.OnCollision(collisionInfo);
        }
    }
}