using MonoGame.Extended.Entities;

namespace Platformer.Components
{
    public enum Facing
    {
        Left, Right
    }

    public enum State
    {
        Idle,
        Attacking,
        Jumping,
        Falling,
        Walking,
        Cool
    }

    [EntityComponent]
    public class Player
    {
        public Facing Facing { get; set; } = Facing.Right;
        public State State { get; set; }
    }
}