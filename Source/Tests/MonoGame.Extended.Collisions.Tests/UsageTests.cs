using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    /// <summary>
    /// Simple usage tests showing how to use this library.
    /// </summary>
    [TestFixture]
    [Ignore("This test currently fails.")]
    public class UsageTests
    {
        /// <summary>
        /// This is a basic test to make sure two actors collide in a <see cref="CollisionWorld"/>
        /// </summary>
        [Test]
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
            Assert.AreEqual(actor1.CollisionCount, actor2.CollisionCount);
            Assert.AreEqual(1, actor1.CollisionCount);
        }
    }
}