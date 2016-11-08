using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Components
{
    public class EnemyAi : EntityComponent
    {
        public EnemyAi()
        {
            Direction = new Vector2(30, 0);
            WalkTime = 4.0f;
            WalkTimeRemaining = WalkTime;
        }

        public Vector2 Direction { get; set; }
        public float WalkTime { get; set; }
        public float WalkTimeRemaining { get; set; }
    }
}