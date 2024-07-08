using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public sealed class Matrix3x2Tests
{
    [Fact]
    public void ConstructorTest()
    {
        Vector2 x = new Vector2(1, 2);
        Vector2 y = new Vector2(3, 4);
        Vector2 z = new Vector2(5, 6);

        var matrix = new Matrix3x2(x.X, x.Y, y.X, y.Y, z.X, z.Y);

        Assert.Equal(x, matrix.X);
        Assert.Equal(y, matrix.Y);
        Assert.Equal(z, matrix.Z);
    }
}
