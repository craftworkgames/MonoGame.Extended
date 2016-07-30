using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Triangulation
{
    public class EarClipPolygonTriangulator : IPolygonTriangulator
    {
        private IReadOnlyList<Vector2> _points;
        private readonly List<int> _previousPointIndices;
        private readonly List<int> _nextPointIndices;

        internal EarClipPolygonTriangulator()
        {
            _previousPointIndices = new List<int>();
            _nextPointIndices = new List<int>();
        }

        public void Triangulate(IReadOnlyList<Vector2> points, PlanarTriangleOutputDelegate planarTriangleOutputDelegate)
        {
            // implementation derived from pseudocode taken from:
            // Real Time Collision Detection by Christer Ericson (Morgan Kaufmann, 2005)
            // Chapter 12, page 498-499

            _points = points;


            // setup previous and next links to effectively form a double linked vertex list
            _previousPointIndices.Clear();
            _nextPointIndices.Clear();

            var lastIndex = points.Count - 1;
            int i;

            for (i = 0; i < lastIndex; i++)
            {
                _previousPointIndices[i] = i - 1;
                _nextPointIndices[i] = i + 1;
            }

            _previousPointIndices[0] = lastIndex;
            _nextPointIndices[lastIndex] = 0;

            // start at vertex 0
            i = 0;
            var pointsCount = points.Count;
            var triangle = new Triangle2F();

            // keep removing vertices until just a single triangle is left
            while (pointsCount > 3)
            {
                var isEar = true;

                triangle.FirstPoint = _points[_previousPointIndices[i]];
                triangle.SecondPoint = _points[i];
                triangle.ThirdPoint = _points[_nextPointIndices[i]];

                // vertex B must be convex for the triangle to be an ear
                // for clarity see see http://www.mathopenref.com/coordtrianglearea.html
                if (triangle.GetDoubleSignedArea() > 0)
                {
                    var k = _nextPointIndices[_nextPointIndices[i]];

                    // no other points can be inside the triangle for the triangle to be an ear
                    // check if any of the other points are inside the triangle
                    do
                    {
                        if (triangle.Contains(_points[k]))
                        {
                            // point is in the triangle => not an ear
                            isEar = false;
                            break;
                        }

                        k = _nextPointIndices[k];
                    }
                    while (k != _previousPointIndices[i]);
                }

                if (isEar)
                {
                    planarTriangleOutputDelegate(ref triangle);

                    // "delete" point by redirecting next and previous links
                    _nextPointIndices[_previousPointIndices[i]] = _nextPointIndices[i];
                    _previousPointIndices[_nextPointIndices[i]] = _previousPointIndices[i];
                    pointsCount--;

                    // visit the previous point next
                    i = _previousPointIndices[i];
                }
                else
                {
                    i = _nextPointIndices[i];
                }
            }
        }
    }
}
