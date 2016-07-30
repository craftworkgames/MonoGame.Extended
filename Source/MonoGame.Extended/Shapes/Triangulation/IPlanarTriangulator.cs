using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Triangulation
{
    public interface IPlanarTriangulator
    {
        void Triangulate(IReadOnlyList<Vector2> points, PlanarTriangleOutputDelegate planarTriangleOutputDelegate);
    }
}
