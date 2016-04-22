using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public interface ISpriteDrawContext : IDrawContext
    {
        Texture2D Texture { get; }
    }
}
