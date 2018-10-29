using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public class CollisionActor : IActorTarget
    {
        private readonly IActorTarget _target;
        private Vector2 _position;

        public CollisionActor(IActorTarget target)
        {
            _target = target;
        }

        public string Handle
        {
            get { return _target.Handle; }
        }

        public Vector2 Velocity
        {
            get { return _target.Velocity; }
        }

        public int Tile
        {
            get { return _target.Tile; }
            set { _target.Tile = value; }
        }

        public Vector2 Position
        {
            get { return _target.Position; }
            set { }
        }

        public RectangleF BoundingBox
        {
            get { return new RectangleF(new Point2(_position.X, _position.Y), _target.BoundingBox.Size); }
        }

        public void SetPosition(Vector2 velocity)
        {
            velocity.X = velocity.X > 0 ? 1 * _target.BoundingBox.Size.Width : velocity.X;
            velocity.Y = velocity.Y > 0 ? 1 * _target.BoundingBox.Size.Height : velocity.Y;
            velocity.X = velocity.X < 0 ? -1 * _target.BoundingBox.Size.Width : velocity.X;
            velocity.Y = velocity.Y < 0 ? -1 * _target.BoundingBox.Size.Height : velocity.Y;
            _position = Position + velocity;
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            _target.OnCollision(collisionInfo);
        }
    }
}