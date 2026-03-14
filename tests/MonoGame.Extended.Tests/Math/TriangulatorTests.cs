using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Triangulation;
using Xunit;

namespace MonoGame.Extended.Tests;

public class TriangulatorTests
{
    // Simple clockwise square (mathematical convention, Y-up)
    private static readonly Vector2[] _cwSquare =
    [
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
    ];

    // Simple counter-clockwise square
    private static readonly Vector2[] _ccwSquare =
    [
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1),
    ];

    /// <summary>
    /// Creates a 5-point star polygon with alternating outer and inner vertices.
    /// A star has exactly 5 clockwise turns and 5 counter-clockwise turns, which
    /// caused the old angle-counting algorithm to produce incorrect results.
    /// </summary>
    private static Vector2[] CreateStarVertices(float outerRadius = 100f, float innerRadius = 40f)
    {
        var vertices = new Vector2[10];
        for (int i = 0; i < 5; i++)
        {
            float outerAngle = (float)(Math.PI / 2.0 + i * 2.0 * Math.PI / 5.0);
            float innerAngle = outerAngle + (float)(Math.PI / 5.0);
            vertices[i * 2] = new Vector2(
                (float)(outerRadius * Math.Cos(outerAngle)),
                (float)(outerRadius * Math.Sin(outerAngle)));
            vertices[i * 2 + 1] = new Vector2(
                (float)(innerRadius * Math.Cos(innerAngle)),
                (float)(innerRadius * Math.Sin(innerAngle)));
        }
        return vertices;
    }

    private static Vector2[] Reverse(Vector2[] vertices)
    {
        var reversed = new Vector2[vertices.Length];
        reversed[0] = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
            reversed[i] = vertices[vertices.Length - i];
        return reversed;
    }

    [Fact]
    public void DetermineWindingOrder_ClockwiseSquare_ReturnsClockwise()
    {
        Assert.Equal(WindingOrder.Clockwise, Triangulator.DetermineWindingOrder(_cwSquare));
    }

    [Fact]
    public void DetermineWindingOrder_CounterClockwiseSquare_ReturnsCounterClockwise()
    {
        Assert.Equal(WindingOrder.CounterClockwise, Triangulator.DetermineWindingOrder(_ccwSquare));
    }

    [Fact]
    public void DetermineWindingOrder_ReversedPolygon_ReturnsOppositeOrder()
    {
        Assert.NotEqual(
            Triangulator.DetermineWindingOrder(_cwSquare),
            Triangulator.DetermineWindingOrder(Reverse(_cwSquare)));
    }

    /// <summary>
    /// Regression test for issue #791. A 5-point star has equal CW and CCW turns,
    /// causing the old angle-counting algorithm to return the same winding order
    /// for both the star and its reverse. The shoelace formula fixes this.
    /// </summary>
    [Fact]
    public void DetermineWindingOrder_StarPolygon_ReturnsOppositeForReverse()
    {
        var star = CreateStarVertices();
        var starReversed = Reverse(star);

        var starOrder = Triangulator.DetermineWindingOrder(star);
        var starReversedOrder = Triangulator.DetermineWindingOrder(starReversed);

        Assert.NotEqual(starOrder, starReversedOrder);
    }
}
