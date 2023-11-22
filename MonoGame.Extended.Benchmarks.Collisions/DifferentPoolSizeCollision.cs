using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

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


    public int UpdateCount { get; set; } = 10;


    private List<Collider> _colliders = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        for (int i = 0; i < N; i++)
        {
            var collider = new Collider(new Point2(
                _random.Next(COMPONENT_BOUNDARY_SIZE),
                _random.Next(COMPONENT_BOUNDARY_SIZE)))
            {
                Shift = new Vector2(
                    _random.Next(4) - 2,
                    _random.Next(4) - 2),
            };
            _colliders.Add(collider);
            _collisionComponent.Insert(collider);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        foreach (var collider in _colliders)
            _collisionComponent.Remove(collider);
        _colliders.Clear();
    }

    [Benchmark]
    public void Benchmark()
    {
        for (int i = 0; i < UpdateCount; i++)
        {
            foreach (var collider in _colliders)
                collider.Position += collider.Shift;
            _collisionComponent.Update(_gameTime);
        }
    }
}
