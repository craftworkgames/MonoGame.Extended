using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Curves
{
    public interface IPathF 
    {
        Vector2 StartPoint { get; set; }
        Vector2 EndPoint { get; set; }
        float Length { get; }
    }
}