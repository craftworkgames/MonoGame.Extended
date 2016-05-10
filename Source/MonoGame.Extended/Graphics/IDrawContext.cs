using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IDrawContext
    {
        bool NeedsToApplyChanges { get; }
        void Apply(out Effect effect);
    }
}
