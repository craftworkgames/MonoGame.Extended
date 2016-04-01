namespace MonoGame.Extended.Graphics
{
    public interface IDrawContext
    {
        int PassesCount { get; }
        void ApplyPass(int passIndex);
    }
}
