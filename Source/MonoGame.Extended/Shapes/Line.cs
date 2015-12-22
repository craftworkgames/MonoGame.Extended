using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class Line
    {
        public Line(Vector2 vertex0, Vector2 vertex1)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
        }

        public Vector2 Vertex0 { get; private set; }
        public Vector2 Vertex1 { get; private set; }
    }
}