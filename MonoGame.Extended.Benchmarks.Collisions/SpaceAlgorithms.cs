using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Benchmarks.Collisions.Utils;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Benchmarks.Collisions;

[SimpleJob(RunStrategy.ColdStart, launchCount:10)]
public class SpaceAlgorithms
{
    private const int COMPONENT_BOUNDARY_SIZE = 1000;

    private readonly Random _random = new ();
    private ISpaceAlgorithm _space;
    private ICollisionActor _actor;
    private RectangleF _bound;
    private List<Collider> _colliders = new();

    [Params(10, 100, 1000)]
    public int N { get; set; }

    [Params("SpatialHash", "QuadTree")]
    public string Algorithm { get; set; }


    [GlobalSetup]
    public void GlobalSetup()
    {
        var size = new Size2(COMPONENT_BOUNDARY_SIZE, COMPONENT_BOUNDARY_SIZE);
        _space = Algorithm switch
        {
            "SpatialHash" => new SpatialHash(new Size2(32, 32)),
            "QuadTree" => new QuadTreeSpace(new RectangleF(Point2.Zero, size)),
            _ => _space
        };
        for (int i = 0; i < N; i++)
        {

            var rect = GetRandomRectangleF();
            var actor = new Collider(rect);
            _colliders.Add(actor);
            _space.Insert(actor);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        foreach (var collider in _colliders)
            _space.Remove(collider);
        _colliders.Clear();
    }

    [GlobalSetup(Targets = new[] { nameof(Insert), nameof(Remove) })]
    public void ActorGlobalSetup()
    {
        GlobalSetup();
        var rect = GetRandomRectangleF();
        _actor = new Collider(rect);
    }

    [Benchmark]
    public void Insert()
    {
        _space.Insert(_actor);
    }

    [Benchmark]
    public void Remove()
    {
        _space.Remove(_actor);
    }

    [Benchmark]
    public void Reset()
    {
        _space.Reset();
    }

    [GlobalSetup(Target = nameof(Query))]
    public void QueryGlobalSetup()
    {
        GlobalSetup();
        _bound = GetRandomRectangleF();
    }

    private RectangleF GetRandomRectangleF()
    {
        return new RectangleF(
            _random.Next(COMPONENT_BOUNDARY_SIZE),
            _random.Next(COMPONENT_BOUNDARY_SIZE),
            _random.Next(32, 128),
            _random.Next(32, 128));
    }

    [Benchmark]
    public List<ICollisionActor> Query()
    {
        return _space.Query(_bound).ToList();
    }
}
