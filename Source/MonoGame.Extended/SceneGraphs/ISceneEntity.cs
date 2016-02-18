using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.SceneGraphs
{
    public interface ISceneEntity
    {
        RectangleF GetBoundingRectangle();
    }
}