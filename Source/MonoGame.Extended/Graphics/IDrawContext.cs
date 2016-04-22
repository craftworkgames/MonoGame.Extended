using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IDrawContext
    {
        bool NeedsUpdate { get; }
        void Apply(out Effect effect);
    }
}
