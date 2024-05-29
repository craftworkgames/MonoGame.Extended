using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests;

public class MathExtendedTests
{
    [Fact]
    public void CalculateMinimumVector2_Returns_Expected()
    {
        Vector2 a = new Vector2(1, 4);
        Vector2 b = new Vector2(3, 2);

        Vector2 expected = new Vector2(1, 2);
        Vector2 actual = MathExtended.CalculateMinimumVector2(a, b);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CalculateMinimumVector2_Out_Expected()
    {
        Vector2 a = new Vector2(1, 4);
        Vector2 b = new Vector2(3, 2);

        Vector2 expected = new Vector2(1, 2);
        MathExtended.CalculateMinimumVector2(a, b, out Vector2 actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CalculateMaximumVector2_Returns_Expected()
    {
        Vector2 a = new Vector2(1, 4);
        Vector2 b = new Vector2(3, 2);

        Vector2 expected = new Vector2(3, 4);
        Vector2 actual = MathExtended.CalculateMaximumVector2(a, b);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CalculateMaximumVector2_Out_Expected()
    {
        Vector2 a = new Vector2(1, 4);
        Vector2 b = new Vector2(3, 2);

        Vector2 expected = new Vector2(3, 4);
        MathExtended.CalculateMaximumVector2(a, b, out Vector2 actual);

        Assert.Equal(expected, actual);
    }
}

