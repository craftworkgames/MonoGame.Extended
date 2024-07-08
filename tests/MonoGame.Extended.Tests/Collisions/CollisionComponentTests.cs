using System;
using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests
{
    using MonoGame.Extended.Collisions.Layers;

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
            _collisionComponent = new CollisionComponent(new RectangleF(Vector2.Zero, new Vector2(10, 10)));
        }

        #region Circle Circle

        [Fact]
        public void PenetrationVectorSameCircleTest()
        {
            Vector2 pos1 = Vector2.Zero;
            Vector2 pos2 = Vector2.Zero;

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
            Vector2 pos1 = new Vector2(0, 1.5f);
            Vector2 pos2 = Vector2.Zero;

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
            Vector2 pos1 = new Vector2(2, 2.5f);
            Vector2 pos2 = new Vector2(2, 1);

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
            Vector2 pos1 = new Vector2(0, 1.5f);
            Vector2 pos2 = Vector2.Zero;

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
            Vector2 pos1 = new Vector2(0, 1);
            Vector2 pos2 = new Vector2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new RectangleF(pos2, new SizeF(4, 2));


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
            Vector2 pos1 = new Vector2(0, 0);
            Vector2 pos2 = new Vector2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 1.0f);
            IShapeF shape2 = new RectangleF(pos2, new SizeF(4, 2));


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
            Vector2 pos1 = new Vector2(2, 1);
            Vector2 pos2 = new Vector2(-2, -1);

            IShapeF shape1 = new CircleF(pos1, 2.0f);
            IShapeF shape2 = new RectangleF(pos2, new SizeF(4, 2));


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
            Vector2 pos1 = new Vector2(0, 0);
            Vector2 pos2 = new Vector2(-2, -1);

            IShapeF shape1 = new RectangleF(pos1, new SizeF(4, 2));
            IShapeF shape2 = new RectangleF(pos2, new SizeF(4, 2));


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
            Vector2 pos1 = new Vector2(4, 2);
            Vector2 pos2 = new Vector2(3, 1);

            IShapeF shape1 = new RectangleF(pos1, new SizeF(4, 2));
            IShapeF shape2 = new RectangleF(pos2, new SizeF(4, 2));


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
                var staticBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
                var anotherStaticBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
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
                var staticBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
                var dynamicBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(staticActor);
                _collisionComponent.Insert(dynamicActor);
                dynamicActor.MoveTo(new Vector2(2, 2));

                _collisionComponent.Update(_gameTime);

                Assert.False(staticActor.IsColliding);
                Assert.False(dynamicActor.IsColliding);
            }

            [Fact]
            public void Actors_is_colliding_when_dynamic_actor_is_moved_after_update()
            {
                var staticBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                _collisionComponent.Insert(staticActor);
                for (int i = 0; i < QuadTree.QuadTree.DefaultMaxObjectsPerNode; i++)
                {
                    var fillerBounds = new RectangleF(new Vector2(0, 2), new SizeF(.1f, .1f));
                    var fillerActor = new CollisionIndicatingActor(fillerBounds);
                    _collisionComponent.Insert(fillerActor);
                }

                var dynamicBounds = new RectangleF(new Vector2(2, 2), new SizeF(1, 1));
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(dynamicActor);

                _collisionComponent.Update(_gameTime);
                Assert.False(staticActor.IsColliding);
                Assert.False(dynamicActor.IsColliding);

                dynamicActor.MoveTo(new Vector2(0, 0));

                _collisionComponent.Update(_gameTime);
                Assert.True(dynamicActor.IsColliding);
                Assert.True(staticActor.IsColliding);
            }

            [Fact]
            public void Actors_is_colliding_when_dynamic_actor_is_moved_into_collision_bounds()
            {
                var staticBounds = new RectangleF(new Vector2(0, 0), new SizeF(1, 1));
                var dynamicBounds = new RectangleF(new Vector2(2, 2), new SizeF(1, 1));
                var staticActor = new CollisionIndicatingActor(staticBounds);
                var dynamicActor = new CollisionIndicatingActor(dynamicBounds);
                _collisionComponent.Insert(staticActor);
                _collisionComponent.Insert(dynamicActor);
                dynamicActor.MoveTo(new Vector2(0, 0));

                _collisionComponent.Update(_gameTime);

                Assert.True(staticActor.IsColliding);
                Assert.True(dynamicActor.IsColliding);
            }

            [Fact]
            public void InsertActor_ThrowsUndefinedLayerException_IfThereIsNoLayerDefined()
            {
                var sut = new CollisionComponent();

                var act = () => sut.Insert(new CollisionIndicatingActor(RectangleF.Empty));

                Assert.Throws<UndefinedLayerException>(act);
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

                public void MoveTo(Vector2 position)
                {
                    _bounds = new RectangleF(position, _bounds.Size);
                }
            }
        }
    }
}
