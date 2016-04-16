using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IEffectDrawContext : IDrawContext
    {
        Effect Effect { get; }
    }
}
