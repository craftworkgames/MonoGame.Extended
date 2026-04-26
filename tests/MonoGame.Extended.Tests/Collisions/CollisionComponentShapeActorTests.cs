using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class CollisionComponentShapeActorTests
    {
        private readonly CollisionComponent _collisionComponent;
        private readonly GameTime _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

        public CollisionComponentShapeActorTests()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(Vector2.Zero, new Vector2(10, 10)));
        }

        [Fact]
        public void ShapeActors_WithOverlappingBoxes_Collide()
        {
            ShapeCollisionActor actor = new ShapeCollisionActor(
                new CollisionShape2D(new BoundingBox2D(new Vector2(0.0f, 0.0f), new Vector2(2.0f, 2.0f))));
            ShapeCollisionActor other = new ShapeCollisionActor(
                new CollisionShape2D(new BoundingBox2D(new Vector2(1.0f, 0.0f), new Vector2(3.0f, 2.0f))));

            _collisionComponent.Insert(actor);
            _collisionComponent.Insert(other);

            _collisionComponent.Update(_gameTime);

            Assert.True(actor.IsColliding);
            Assert.True(other.IsColliding);
            Assert.NotEqual(Vector2.Zero, actor.PenetrationVector);
            Assert.NotEqual(Vector2.Zero, other.PenetrationVector);
        }

        [Fact]
        public void ShapeActors_WithOverlappingCircleAndBox_Collide()
        {
            ShapeCollisionActor actor = new ShapeCollisionActor(
                new CollisionShape2D(new BoundingCircle2D(new Vector2(0.0f, 0.0f), 2.0f)));
            ShapeCollisionActor other = new ShapeCollisionActor(
                new CollisionShape2D(new BoundingBox2D(new Vector2(1.0f, -2.0f), new Vector2(5.0f, 2.0f))));

            _collisionComponent.Insert(actor);
            _collisionComponent.Insert(other);

            _collisionComponent.Update(_gameTime);

            Assert.True(actor.IsColliding);
            Assert.True(other.IsColliding);
            Assert.NotEqual(Vector2.Zero, actor.PenetrationVector);
            Assert.NotEqual(Vector2.Zero, other.PenetrationVector);
        }

        [Fact]
        public void ShapeActors_WithOverlappingOrientedBoxAndBox_Collide()
        {
            ShapeCollisionActor actor = new ShapeCollisionActor(
                new CollisionShape2D(OrientedBoundingBox2D.CreateFromRotation(
                    new Vector2(3.0f, 2.0f),
                    MathHelper.PiOver4,
                    new Vector2(2.0f, 2.0f))));
            ShapeCollisionActor other = new ShapeCollisionActor(
                new CollisionShape2D(new BoundingBox2D(new Vector2(0.0f, 0.0f), new Vector2(4.0f, 4.0f))));

            _collisionComponent.Insert(actor);
            _collisionComponent.Insert(other);

            _collisionComponent.Update(_gameTime);

            Assert.True(actor.IsColliding);
            Assert.True(other.IsColliding);
            Assert.NotEqual(Vector2.Zero, actor.PenetrationVector);
            Assert.NotEqual(Vector2.Zero, other.PenetrationVector);
        }

        private sealed class ShapeCollisionActor : ICollisionActor
        {
            private static int _nextId = 1;

            public ShapeCollisionActor(CollisionShape2D shape)
            {
                Shape = shape;
            }

            public int Id { get; } = _nextId++;

            public string LayerName => null;

            public CollisionShape2D Shape { get; }

            public bool IsColliding { get; private set; }

            public Vector2 PenetrationVector { get; private set; }

            public void OnCollision(CollisionEventArgs collisionInfo)
            {
                IsColliding = true;
                PenetrationVector = collisionInfo.PenetrationVector;
            }
        }
    }
}
