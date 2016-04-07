using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Entities
{
    public class Entity
    {
        public VisualComponent VisualComponent { get; set; }
        public CollisionComponent CollisionComponent { get; set; }
        public TransformComponent TransformComponent { get; set; }

    }

    public abstract class TransformComponent
    {
        public abstract Matrix TransformMatrix { get; }

    }

    public abstract class CollisionComponent
    {
        public abstract CollisionInfo[] CheckCollide();
    }

    public abstract class VisualComponent
    {
        public abstract void Draw(Vector2 position, Vector2 scale, float rotation);
    }
}