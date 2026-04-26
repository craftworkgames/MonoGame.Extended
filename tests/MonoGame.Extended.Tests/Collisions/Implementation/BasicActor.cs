using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicActor : ICollisionActor
    {
        private enum TestShapeKind
        {
            Box,
            Circle,
            OrientedBox
        }

        private static int _nextId = 1;

        private TestShapeKind _shapeKind;
        private BoundingBox2D _boxBounds;
        private BoundingCircle2D _circleBounds;
        private OrientedBoundingBox2D _orientedBoxBounds;

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

        public int Id { get; } = _nextId++;

        public Vector2 Position { get; private set; }

        public CollisionShape2D Shape { get; private set; }

        public Vector2 Velocity { get; set; }

        public int CollisionCount { get; set; }

        public void SetBounds(BoundingBox2D bounds)
        {
            _shapeKind = TestShapeKind.Box;
            _boxBounds = bounds;
            Position = bounds.Min;
            Shape = new CollisionShape2D(bounds);
        }

        public void SetBounds(BoundingCircle2D bounds)
        {
            _shapeKind = TestShapeKind.Circle;
            _circleBounds = bounds;
            Position = bounds.Center;
            Shape = new CollisionShape2D(bounds);
        }

        public void SetBounds(OrientedBoundingBox2D bounds)
        {
            _shapeKind = TestShapeKind.OrientedBox;
            _orientedBoxBounds = bounds;
            Position = bounds.Center;
            Shape = new CollisionShape2D(bounds);
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            Translate(-collisionInfo.PenetrationVector);
            if (collisionInfo.Other is BasicActor)
                CollisionCount++;
        }

        private void Translate(Vector2 translation)
        {
            Position += translation;

            switch (_shapeKind)
            {
                case TestShapeKind.Box:
                    _boxBounds.Min += translation;
                    _boxBounds.Max += translation;
                    Shape = new CollisionShape2D(_boxBounds);
                    break;
                case TestShapeKind.Circle:
                    _circleBounds.Center += translation;
                    Shape = new CollisionShape2D(_circleBounds);
                    break;
                case TestShapeKind.OrientedBox:
                    _orientedBoxBounds.Center += translation;
                    Shape = new CollisionShape2D(_orientedBoxBounds);
                    break;
            }
        }
    }
}
