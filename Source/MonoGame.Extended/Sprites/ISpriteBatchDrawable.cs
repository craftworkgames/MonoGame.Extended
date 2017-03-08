using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public interface ISpriteBatchDrawable
    {
        bool IsVisible { get; }
        TextureRegion2D TextureRegion { get; }
        Vector2 Position { get; }
        float Rotation { get; }
        Vector2 Scale { get; }
        Color Color { get; }
        Vector2 Origin { get; }
        SpriteEffects Effect { get; }
    }
}