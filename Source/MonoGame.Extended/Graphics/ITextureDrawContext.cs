using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface ITextureDrawContext : IDrawContext
    {
        Texture Texture { get; set; }
    }
}
