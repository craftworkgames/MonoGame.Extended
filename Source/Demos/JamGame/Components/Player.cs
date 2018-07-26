namespace JamGame.Components
{
    public enum Facing
    {
        Left, Right
    }

    public class Player
    {
        private Facing Facing { get; set; } = Facing.Right;
    }
}