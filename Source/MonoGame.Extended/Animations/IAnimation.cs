namespace MonoGame.Extended.Animations
{
    public interface IAnimation : IUpdate
    {
        bool IsComplete { get; }
    }
}