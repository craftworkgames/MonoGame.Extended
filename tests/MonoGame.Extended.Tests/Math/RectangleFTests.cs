namespace MonoGame.Extended.Tests;

public sealed class RectangleFTests
{
    [Fact]
    public void Normalize_WithNegativeWidthAndHeight_AdjustsPositionAndMakesDimensionsPositive()
    {
        RectangleF rectangle = new RectangleF(32, 32, -32, -32);
        rectangle.Normalize();

        RectangleF expected = new RectangleF(0, 0, 32, 32);
        Assert.Equal(expected, rectangle);
    }

    [Fact]
    public void Normalize_WithNegativeWidthOnly_AdjustsXAndMakesWidthPositive()
    {
        RectangleF rectangle = new RectangleF(100, 50, -20, 30);
        rectangle.Normalize();

        RectangleF expected = new RectangleF(80, 50, 20, 30);
        Assert.Equal(expected, rectangle);
    }

    [Fact]
    public void Normalize_WithNegativeHeightOnly_AdjustsYAndMakesHeightPositive()
    {
        RectangleF rectangle = new RectangleF(50, 100, 30, -20);
        rectangle.Normalize();

        RectangleF expected = new RectangleF(50, 80, 30, 20);
        Assert.Equal(expected, rectangle);
    }

    [Fact]
    public void Normalize_WithPositiveDimensions_DoesNotModifyRectangle()
    {
        RectangleF rectangle = new RectangleF(10, 20, 30, 40);
        rectangle.Normalize();

        RectangleF expected = new RectangleF(10, 20, 30, 40);
        Assert.Equal(expected, rectangle);
    }

    [Fact]
    public void Normalize_StaticMethod_ReturnsNormalizedRectangle()
    {
        RectangleF rectangle = new RectangleF(32, 32, -32, -32);

        RectangleF actual = RectangleF.Normalize(rectangle);
        RectangleF expected = new RectangleF(0, 0, 32, 32);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Normalize_RefOutMethod_OutputsNormalizedRectangle()
    {
        RectangleF rectangle = new RectangleF(32, 32, -32, -32);

        RectangleF.Normalize(ref rectangle, out RectangleF actual);
        RectangleF expected = new RectangleF(0, 0, 32, 32);

        Assert.Equal(expected, actual);
    }

    [Trait("Issue", "#793")]
    public sealed class RectangleFIssue747
    {
        [Fact]
        public void Normalize_FixesIntersectionIssue_FromGitHubIssue747()
        {
            // This was the original reported issue
            RectangleF shape1 = new RectangleF(32, 32, -32, -32);
            RectangleF shape2 = new RectangleF(16, 16, -32, -32);

            shape1.Normalize();
            shape2.Normalize();

            Assert.True(shape1.Intersects(shape2));
        }
    }
}
