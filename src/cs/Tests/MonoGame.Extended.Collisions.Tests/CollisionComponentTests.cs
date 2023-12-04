using System;
using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests
{
    /// <summary>
    /// Test collision of actors with various shapes.
    /// </summary>
    /// <remarks>
    /// Uses the fact that <see cref="BasicActor"/> moves itself away from
    /// <see cref="BasicWall"/> on collision.
    /// </remarks>
    public class CollisionComponentTests
    {
        private readonly CollisionComponent _collisionComponent;
        private readonly GameTime _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

        public CollisionComponentTests()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(Point2.Zero, new Point2(10, 10)));
        }

        #region Circle Circle

        [Fact]
        public void PenetrationVectorSameCircleTest()
        {
            Point2 pos1 = Point2.Zero;
            Point2 pos2 = Point2.Zero;

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new CircleF(pos2, 2.0f);

            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.Y - -4f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationVectorSlightlyOverlappingCircleTest()
        {
            Point2 pos1 = new Point2(0, 1.5f);
            Point2 pos2 = Point2.Zero;

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new CircleF(pos2, 2.0f);

            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            // Actor should have moved up because the distance is shorter.
            Assert.True(actor1.Position.Y > actor2.Position.Y);
            // The circle centers should be about 4 units away after moving
            Assert.True(Math.Abs(actor1.Position.Y - 4.0f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationVectorSlightlyOverlappingOffAxisTest()
        {
            Point2 pos1 = new Point2(2, 2.5f);
            Point2 pos2 = new Point2(2, 1);

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new CircleF(pos2, 2.0f);

            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            // Actor should have moved up because the distance is shorter.
            Assert.True(actor1.Position.Y > actor2.Position.Y);
            // The circle centers should be about 4 units away after moving
            Assert.True(Math.Abs(actor1.Position.Y - 5.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.X - 2.0f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationZeroRadiusCircleCircleTest()
        {
            Point2 pos1 = new Point2(0, 1.5f);
            Point2 pos2 = Point2.Zero;

            IShapeF shape1 = new CircleF(pos1, 0);
            IShapeF shape2 = new CircleF(pos2, 2.0f);

            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            // Actor should have moved up because the distance is shorter.
            Assert.True(actor1.Position.Y > actor2.Position.Y);
            // The circle centers should be about 4 units away after moving
            // Assert.True(Math.Abs(actor1.Position.Y - 2.0f) < float.Epsilon);
        }

        #endregion

        #region Circle Rectangle

        [Fact]
        public void PenetrationVectorCircleRectangleTest()
        {
            Point2 pos1 = new Point2(0, 1);
            Point2 pos2 = new Point2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new RectangleF(pos2, new Size2(4, 2));


            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.X - 0.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.Y - 3.0f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationVectorCircleContainedInRectangleTest()
        {
            Point2 pos1 = new Point2(0, 0);
            Point2 pos2 = new Point2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 1.0f);
            IShapeF shape2 = new RectangleF(pos2, new Size2(4, 2));


            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.X - 0.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.Y - -2.0f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationVectorCircleOffAxisRectangleTest()
        {
            Point2 pos1 = new Point2(2, 1);
            Point2 pos2 = new Point2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new RectangleF(pos2, new Size2(4, 2));


            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.X - 2.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.Y - 3.0f) < float.Epsilon);
        }

        #endregion

        #region Rectangle Rectangle

        [Fact]
        public void PenetrationVectorRectangleRectangleTest()
        {
            Point2 pos1 = new Point2(0, 0);
            Point2 pos2 = new Point2(-2, -1);

            IShapeF shape1 = new RectangleF(pos1, new Size2(4, 2));
            IShapeF shape2 = new RectangleF(pos2, new Size2(4, 2));


            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.X - 0.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.Y - 1.0f) < float.Epsilon);
        }

        [Fact]
        public void PenetrationVectorRectangleRectangleOffAxisTest()
        {
            Point2 pos1 = new Point2(4, 2);
            Point2 pos2 = new Point2(3, 1);

            IShapeF shape1 = new RectangleF(pos1, new Size2(4, 2));
            IShapeF shape2 = new RectangleF(pos2, new Size2(4, 2));


            var actor1 = new BasicActor()
            {
                Position = pos1,
                Bounds = shape1
            };
            var actor2 = new BasicWall()
            {
                Position = pos2,
                Bounds = shape2
            };

            Assert.True(shape1.Intersects(shape2));
            _collisionComponent.Insert(actor1);
            _collisionComponent.Insert(actor2);
            _collisionComponent.Update(_gameTime);
            Assert.True(Math.Abs(actor1.Position.X - 4.0f) < float.Epsilon);
            Assert.True(Math.Abs(actor1.Position.Y - 3.0f) < float.Epsilon);
        }

        #endregion

        public class Behaviours : CollisionComponentTests
        {
            [Fact]
            public void Actors_is_colliding()
            {
                var staticBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var anotherStaticBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                var anotherStaticActor = new CollisionIndicatingActor(anotherStaticBounds);
                _collisionComponent.Insert(staticActor);
                _collisionComponent.Insert(anotherStaticActor);

                _collisionComponent.Update(_gameTime);

                Assert.True(staticActor.IsColliding);
                Assert.True(anotherStaticActor.IsColliding);
            }

            [Fact]
            public void Actors_is_not_colliding_when_dynamic_actor_is_moved_out_of_collision_bounds()
            {
                var staticBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var dynamicBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(staticActor);
                _collisionComponent.Insert(dynamicActor);
                dynamicActor.MoveTo(new Point2(2, 2));

                _collisionComponent.Update(_gameTime);

                Assert.False(staticActor.IsColliding);
                Assert.False(dynamicActor.IsColliding);
            }

            [Fact]
            public void Actors_is_colliding_when_dynamic_actor_is_moved_after_update()
            {
                var staticBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                _collisionComponent.Insert(staticActor);
                for (int i = 0; i < QuadTree.QuadTree.DefaultMaxObjectsPerNode; i++)
                {
                    var fillerBounds = new RectangleF(new Point2(0, 2), new Size2(.1f, .1f));
                    var fillerActor = new CollisionIndicatingActor(fillerBounds);
                    _collisionComponent.Insert(fillerActor);
                }

                var dynamicBounds = new RectangleF(new Point2(2, 2), new Size2(1, 1));
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(dynamicActor);

                _collisionComponent.Update(_gameTime);
                Assert.False(staticActor.IsColliding);
                Assert.False(dynamicActor.IsColliding);

                dynamicActor.MoveTo(new Point2(0, 0));

                _collisionComponent.Update(_gameTime);
                Assert.True(dynamicActor.IsColliding);
                Assert.True(staticActor.IsColliding);
            }

            [Fact]
            public void Actors_is_colliding_when_dynamic_actor_is_moved_into_collision_bounds()
            {
                var staticBounds = new RectangleF(new Point2(0, 0), new Size2(1, 1));
                var dynamicBounds = new RectangleF(new Point2(2, 2), new Size2(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(staticActor);
                _collisionComponent.Insert(dynamicActor);
                dynamicActor.MoveTo(new Point2(0, 0));

                _collisionComponent.Update(_gameTime);

                Assert.True(staticActor.IsColliding);
                Assert.True(dynamicActor.IsColliding);
            }

            private class CollisionIndicatingActor : ICollisionActor
            {
                private RectangleF _bounds;

                public CollisionIndicatingActor(RectangleF bounds)
                {
                    _bounds = bounds;
                }

                public IShapeF Bounds => _bounds;

                public void OnCollision(CollisionEventArgs collisionInfo)
                {
                    IsColliding = true;
                }

                public bool IsColliding { get; private set; }

                public void MoveTo(Point2 position)
                {
                    _bounds = new RectangleF(position, _bounds.Size);
                }
            }
        }
    }
}
