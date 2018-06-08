using Xunit;

namespace MonoGame.Extended.Collisions.Tests
{
    public class CollisionComponentTests
    {
        private CollisionComponent collisionComponent;

        public CollisionComponentTests()
        {
            collisionComponent = new CollisionComponent(new RectangleF(Point2.Zero, new Point2(10, 10)));
        }


        [Fact]
        public void PenetrationVectorSameCircleTest()
        {
            IShapeF shape1 = new CircleF(Point2.Zero, 2.0f);
            IShapeF shape2 = new CircleF(Point2.Zero, 2.0f);

            var actor1 = new BasicActor();
        }
    }
}