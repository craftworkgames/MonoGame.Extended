using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [Component]
    [ComponentPool(InitialSize = 100)]
    public class EnemyAiComponent : Component
    {
        public Vector2 Direction { get; set; }
        public float WalkTime { get; set; }
        public float WalkTimeRemaining { get; set; }

        public override void Reset()
        {
            Direction = new Vector2(30, 0);
            WalkTime = 4.0f;
            WalkTimeRemaining = WalkTime;
        }
    }
}