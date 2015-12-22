using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Shapes
{
    [TestFixture]
    public class PolygonFTests
    {
        [Test]
        public void Polygon_Contains_Point_Test()
        {
            var vertices = new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(10, 10),
                new Vector2(0, 10)
            };

            var polygon = new PolygonF(vertices);

            Assert.IsTrue(polygon.Contains(new Vector2(5, 5)));
            Assert.IsTrue(polygon.Contains(new Vector2(0.01f, 0.01f)));
            Assert.IsTrue(polygon.Contains(new Vector2(9.99f, 9.99f)));
            Assert.IsFalse(polygon.Contains(new Vector2(-1f, -1f)));
            Assert.IsFalse(polygon.Contains(new Vector2(-11f, -11f)));
        }

        [Test]
        public void Polygon_Transform_Position_Test()
        {
            var vertices = new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(10, 10),
                new Vector2(0, 10)
            };

            var polygon = new PolygonF(vertices)
                .Transform(new Vector2(2, 3), Vector2.Zero, 0, Vector2.One);

            Assert.AreEqual(new Vector2(2, 3), polygon.Vertices[0]);
            Assert.AreEqual(new Vector2(12, 3), polygon.Vertices[1]);
            Assert.AreEqual(new Vector2(12, 13), polygon.Vertices[2]);
            Assert.AreEqual(new Vector2(2, 13), polygon.Vertices[3]);
        }

        [Test]
        public void Polygon_Transform_Rotation_Test()
        {
            var vertices = new[]
            {
                new Vector2(0, 0),
                new Vector2(5, 0),
                new Vector2(5, 10),
                new Vector2(0, 10)
            };

            var polygon = new PolygonF(vertices)
                .Transform(Vector2.Zero, new Vector2(2.5f, 5.0f), MathHelper.ToRadians(90), Vector2.One);

        //    Assert.AreEqual(new Vector2(2, 3), polygon.Vertices[0]);
        //    Assert.AreEqual(new Vector2(12, 3), polygon.Vertices[1]);
        //    Assert.AreEqual(new Vector2(12, 13), polygon.Vertices[2]);
        //    Assert.AreEqual(new Vector2(2, 13), polygon.Vertices[3]);
        }
    }
}