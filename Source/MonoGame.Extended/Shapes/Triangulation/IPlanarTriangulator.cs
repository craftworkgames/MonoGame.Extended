using System.Collections.Generic;

namespace MonoGame.Extended.Shapes.Triangulation
{
    public interface IPlanarTriangulator
    {
        void Triangulate(IReadOnlyList<Point2F> points, PlanarTriangleOutputDelegate planarTriangleOutputDelegate);
    }
}
