using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;

namespace MonoGame.Extended.Benchmarks.Collisions;

[SimpleJob(RunStrategy.ColdStart, launchCount:3)]
public class DifferentPoolSizeCollision
{
    private const int COMPONENT_BOUNDARY_SIZE = 1000;

    private readonly CollisionComponent _collisionComponent;
    private readonly Random _random = new Random();
    private readonly GameTime _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

    public DifferentPoolSizeCollision()
    {
        var size = new Size2(COMPONENT_BOUNDARY_SIZE, COMPONENT_BOUNDARY_SIZE);
        _collisionComponent = new CollisionComponent(new RectangleF(Point2.Zero, size));
    }

    class Collider: ICollisionActor
    {
        public Collider(Point2 position)
        {
            Bounds = new RectangleF(position, new Size2(1, 1));
        }

        public IShapeF Bounds { get; set; }
        public Vector2 Shift { get; set; }

        public Point2 Position {
            get => Bounds.Position;
            set => Bounds.Position = value;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }

    [Params(100, 500, 1000)]
    public int N { get; set; }


    [Params(1, 2)]
    public int LayersCount { get; set; }

    public int UpdateCount { get; set; } = 100;


    private List<Collider> _colliders = new();
    private List<Layer> _layers = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        if (LayersCount > 1)
        {
            for (int i = 0; i < LayersCount; i++)
            {
                var size = new Size2(COMPONENT_BOUNDARY_SIZE, COMPONENT_BOUNDARY_SIZE);
                var layer = new Layer(new SpatialHash(new Size2(5, 5)));//new QuadTreeSpace(new RectangleF(Point2.Zero, size)))));
                _collisionComponent.Add(i.ToString(), layer);
                _layers.Add(layer);
            }
            for (int i = 0; i < LayersCount - 1; i++)
                _collisionComponent.AddCollisionBetweenLayer(_layers[i], _layers[i + 1]);

        }

        for (int i = 0; i < N; i++)
        {
            var layer = LayersCount == 1
                ? _collisionComponent.Layers.First().Value
                : _layers[i % LayersCount];

            var collider = new Collider(new Point2(
                _random.Next(COMPONENT_BOUNDARY_SIZE),
                _random.Next(COMPONENT_BOUNDARY_SIZE)))
            {
                Shift = new Vector2(
                    _random.Next(4) - 2,
                    _random.Next(4) - 2),
            };
            _colliders.Add(collider);
            layer.Space.Insert(collider);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        foreach (var collider in _colliders)
            _collisionComponent.Remove(collider);
        _colliders.Clear();
        foreach (var layer in _layers)
            _collisionComponent.Remove(layer: layer);
        _layers.Clear();
    }

    [Benchmark]
    public void Benchmark()
    {
        for (int i = 0; i < UpdateCount; i++)
        {
            foreach (var collider in _colliders)
                collider.Position += collider.Shift;
            //_collisionComponent.Update(_gameTime);
        }
    }
}
