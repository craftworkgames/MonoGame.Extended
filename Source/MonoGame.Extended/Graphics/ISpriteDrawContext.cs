using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface ISpriteDrawContext : IDrawContext
    {
        Texture2D Texture { get; }
    }
}
