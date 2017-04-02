using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [EntityComponent]
    [EntityComponentPool(InitialSize = 100)]
    public class CollisionBodyComponent : EntityComponent
    {
        public Vector2 Velocity { get; set; }
        public Size2 Size { get; set; }
        public Vector2 Origin { get; set; }
        public RectangleF BoundingRectangle { get; set; }
        public bool IsStatic { get; set; }

        public event Action<CollisionBodyComponent, CollisionBodyComponent, Vector2> Collision; 

        public override void Reset()
        {
            Velocity = Vector2.Zero;
            Size = Size2.Empty;
            Origin = Vector2.Zero;
            BoundingRectangle = RectangleF.Empty;
            IsStatic = false;
        }

        public void OnCollision(CollisionBodyComponent bodyA, CollisionBodyComponent bodyB, Vector2 depth)
        {
            Collision?.Invoke(bodyA, bodyB, depth);
        }
    }
}