using System;
using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Collisions.Tests
{
    /// <summary>
    /// Simple usage tests showing how to use this library.
    /// </summary>
    public class UsageTests
    {
        /// <summary>
        /// This is a basic test to make sure two actors collide in a <see cref="CollisionWorld"/>
        /// </summary>
        [Fact(Skip = "This test currently fails and needs fixed.")]
        public void BasicUsageTest()
        {
            var world = new CollisionWorld(Vector2.Zero);
            var boundingBox = new RectangleF(2, 2, 10, 10);

            // The two actors are exactly overlapping, so they should collide.
            var actor1 = new BasicActor {BoundingBox = boundingBox};
            var actor2 = new BasicActor {BoundingBox = boundingBox};

            world.CreateActor(actor1);
            world.CreateActor(actor2);

            var data = new[] {1, 1};
            world.CreateGrid(data, 2, 1, 10, 10);

            world.Update(new GameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)));
            

            // The two actors should have collided.
            Assert.Equal(actor1.CollisionCount, actor2.CollisionCount);
            Assert.Equal(1, actor1.CollisionCount);
        }
    }
}