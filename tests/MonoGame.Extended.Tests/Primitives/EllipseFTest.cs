using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests.Primitives
{

    public class EllipseFTest
    {
        [Theory]
        [InlineData(-1, -1, false)]
        [InlineData(110, 300, true)]
        [InlineData(200, 300, true)]
        [InlineData(290, 300, true)]
        [InlineData(400, 400, false)]
        public void ContainsPoint_Circle(int x, int y, bool expected)
        {
            var ellipse = new EllipseF(new Vector2(200.0f, 300.0f), 100.0f, 100.0f);

            Assert.Equal(expected, ellipse.Contains(x, y));
        }

        [Theory]
        [InlineData(299, 400, false)]
        [InlineData(501, 400, false)]
        [InlineData(400, 199, false)]
        [InlineData(400, 601, false)]
        [InlineData(301, 400, true)]
        [InlineData(499, 400, true)]
        [InlineData(400, 201, true)]
        [InlineData(400, 599, true)]
        public void ContainsPoint_NonCircle(int x, int y, bool expected)
        {
            var ellipse = new EllipseF(new Vector2(400.0f, 400.0f), 100.0f, 200.0f);

            Assert.Equal(expected, ellipse.Contains(x, y));
        }
    }
}
