using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Triangulation;

namespace MonoGame.Extended.Benchmarks.Triangulation;

/// <summary>
/// Benchmarks for Triangulator heap allocation (issue #930).
///
/// Shared between Baseline (NuGet 5.3.1) and Source (patched) projects so both
/// run identical benchmark logic against different versions of the library.
///
/// Run Baseline:
///   dotnet run -c Release --project benchmarks/MonoGame.Extended.Benchmarks.Triangulation/Baseline
/// Run Source (after applying fix):
///   dotnet run -c Release --project benchmarks/MonoGame.Extended.Benchmarks.Triangulation/Source
/// </summary>
[MemoryDiagnoser]
public class TriangulatorBenchmarks
{
    // CCW square: the common case driven by DrawSolidRectangle
    private Vector2[] _square = null!;

    // CCW pentagon: small irregular polygon
    private Vector2[] _pentagon = null!;

    // CCW 16-gon: approximates a circle, driven by DrawSolidEllipse
    private Vector2[] _circle16 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Matches the vertex layout produced by DrawSolidRectangle (CCW per shoelace formula)
        _square = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(100, 0),
            new Vector2(100, 100),
            new Vector2(0, 100),
        };

        _pentagon = CreateRegularPolygon(5, 100f);
        _circle16 = CreateRegularPolygon(16, 100f);
    }

    [Benchmark(Baseline = true)]
    public int Triangulate_Square()
    {
        Triangulator.Triangulate(_square, WindingOrder.CounterClockwise, out _, out int[] indices);
        return indices.Length;
    }

    [Benchmark]
    public int Triangulate_Pentagon()
    {
        Triangulator.Triangulate(_pentagon, WindingOrder.CounterClockwise, out _, out int[] indices);
        return indices.Length;
    }

    [Benchmark]
    public int Triangulate_Circle16()
    {
        Triangulator.Triangulate(_circle16, WindingOrder.CounterClockwise, out _, out int[] indices);
        return indices.Length;
    }

    /// <summary>
    /// Creates a regular polygon wound CCW (counterclockwise in shoelace/Y-up convention).
    /// </summary>
    private static Vector2[] CreateRegularPolygon(int sides, float radius)
    {
        var verts = new Vector2[sides];
        for (int i = 0; i < sides; i++)
        {
            double angle = 2.0 * Math.PI * i / sides;
            verts[i] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
        }
        return verts;
    }
}
