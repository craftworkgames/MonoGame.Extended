
namespace Platformer.Components
{
    public enum Facing
    {
        Left, Right
    }

    public enum State
    {
        Idle,
        Kicking,
        Punching,
        Jumping,
        Falling,
        Walking,
        Cool
    }

    public class Player
    {
        public Facing Facing { get; set; } = Facing.Right;
        public State State { get; set; }
        public bool IsAttacking => State == State.Kicking || State == State.Punching;
        public bool CanJump => State == State.Idle || State == State.Walking;
    }
}