using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public class ExplicitShape2D
    {
        // ReSharper disable once InconsistentNaming
        internal readonly Vector2[] _vertices;

        public IReadOnlyList<Vector2> Vertices
        {
            get { return _vertices; }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ExplicitShape2D(Vector2[] vertices)
        {
            _vertices = vertices;
        }

        public Vector2 Project(ref Vector2 axis)
        {
            var min = axis.Dot(_vertices[0]);
            var max = min;

            for (var i = 1; i < _vertices.Length; i++)
            {
                var dotProduct = axis.Dot(_vertices[i]);

                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else if (dotProduct > max)
                {
                    max = dotProduct;
                }
            }

            return new Vector2(min, max);
        }
    }
}
