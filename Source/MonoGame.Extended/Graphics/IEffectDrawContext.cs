using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    internal interface IEffectDrawContext : IDrawContext
    {
        Effect Effect { get; }
    }
}
