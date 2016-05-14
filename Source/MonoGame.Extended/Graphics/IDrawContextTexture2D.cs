using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IDrawContextTexture2D : IDrawContext
    {
        Texture2D Texture { get; }
    }
}
