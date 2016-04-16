namespace MonoGame.Extended.Graphics
{
    public interface IDrawContext
    {
        uint SortKey { get; }
        int PassesCount { get; }

        void Begin();
        void End();
        void ApplyPass(int passIndex);
    }
}
