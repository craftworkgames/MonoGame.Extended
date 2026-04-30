using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests;

public class CollisionEvent2DTests
{
    [Fact]
    public void ConstructorStyleInitialization_WhenInitializedWithOtherActorAndResult_StoresOtherActorAndResult()
    {
        TestCollisionActor other = new TestCollisionActor(42);
        CollisionResult2D result = new CollisionResult2D(true, Vector2.UnitX, 2.0f, new Vector2(2.0f, 0.0f));

        CollisionEvent2D collision = new CollisionEvent2D
        {
            Other = other,
            Result = result
        };

        Assert.Same(other, collision.Other);
        Assert.Equal(42, collision.OtherId);
        Assert.Equal(result, collision.Result);
    }

    private sealed class TestCollisionActor : ICollisionActor
    {
        public TestCollisionActor(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public CollisionShape2D Shape => default;
    }
}
