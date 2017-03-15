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

            var polygon = new Polygon(vertices);

            Assert.IsTrue(polygon.Contains(new Vector2(5, 5)));
            Assert.IsTrue(polygon.Contains(new Vector2(0.01f, 0.01f)));
            Assert.IsTrue(polygon.Contains(new Vector2(9.99f, 9.99f)));
            Assert.IsFalse(polygon.Contains(new Vector2(-1f, -1f)));
            Assert.IsFalse(polygon.Contains(new Vector2(-11f, -11f)));
        }

        [Test]
        public void Polygon_Transform_Translation_Test()
        {
            var vertices = new[] 
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(10, 10),
                new Vector2(0, 10)
            };

            var polygon = new Polygon(vertices);
            polygon.Offset(new Vector2(2, 3));

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
                new Vector2(-5, -5),
                new Vector2(5, 10),
                new Vector2(-5, 10)
            };

            var polygon = new Polygon(vertices);
            polygon.Rotate(MathHelper.ToRadians(90));

            const float tolerance = 0.01f;
            Assert.IsTrue(new Vector2(5, -5).EqualsWithTolerence(polygon.Vertices[0], tolerance));
            Assert.IsTrue(new Vector2(-10, 5).EqualsWithTolerence(polygon.Vertices[1], tolerance));
            Assert.IsTrue(new Vector2(-10, -5).EqualsWithTolerence(polygon.Vertices[2], tolerance));
        }

        [Test]
        public void Polygon_Transform_Scale_Test()
        {
            var vertices = new[]
            {
                new Vector2(0, -1),
                new Vector2(1, 1),
                new Vector2(-1, 1)
            };

            var polygon = new Polygon(vertices);
            polygon.Scale(new Vector2(1, -0.5f));

            const float tolerance = 0.01f;
            Assert.IsTrue(new Vector2(0, -0.5f).EqualsWithTolerence(polygon.Vertices[0], tolerance), "0");
            Assert.IsTrue(new Vector2(2f, 0.5f).EqualsWithTolerence(polygon.Vertices[1], tolerance), "1");
            Assert.IsTrue(new Vector2(-2f, 0.5f).EqualsWithTolerence(polygon.Vertices[2], tolerance), "2");
        }
    }
}