using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.SceneGraphs
{
    public interface ISceneEntity
    {
        RectangleF GetBoundingRectangle();
    }

    public interface ISpriteBatchDrawable
    {
        Color Color { get; }
        SpriteEffects Effect { get; }
        bool IsVisible { get; }
        Vector2 Origin { get; }
        Vector2 Position { get; }
        float Rotation { get; }
        Vector2 Scale { get; }
        TextureRegion2D TextureRegion { get; }
    }
}