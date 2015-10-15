using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionBody : ICollidable
    {
        public CollisionBody(ICollidable target)
        {
            _target = target;
        }

        private readonly ICollidable _target;

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

        public RectangleF GetAxisAlignedBoundingBox()
        {
            return _target.GetAxisAlignedBoundingBox();
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            _target.OnCollision(collisionInfo);
        }
    }
}