using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface IDrawContext
    {
        Effect Effect { get; }
        uint SortKey { get; }

        void Begin();
        void End();
    }
}
