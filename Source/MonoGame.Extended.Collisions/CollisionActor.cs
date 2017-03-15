using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class CollisionActor : IActorTarget
    {
        private readonly IActorTarget _target;

        public CollisionActor(IActorTarget target)
        {
            _target = target;
        }

        public Vector2 Velocity
        {
            get { return _target.Velocity; }
            set { _target.Velocity = value; }
        }

        public Vector2 Position
        {
            get { return _target.Position; }
            set { _target.Position = value; }
        }

        public RectangleF BoundingBox => _target.BoundingBox;

        public void OnCollision(CollisionInfo collisionInfo)
        {
            _target.OnCollision(collisionInfo);
        }
    }
}