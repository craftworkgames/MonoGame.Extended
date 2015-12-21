using System;
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

    public class PolygonF // implements Shape2D
    {
        private Vector2[] _localVertices;
        private Vector2[] _worldVertices;
        private Vector2 _position;
        private Vector2 _origin;
        private float _rotation;
        private Vector2 _scale = Vector2.One;
        private bool _isDirty = true;
        private Rectangle _bounds;

        /** Constructs a new polygon from a float array of parts of vertex points.
         * 
         * @param vertices an array where every even element represents the horizontal part of a point, and the following element
         *           representing the vertical part
         * 
         * @throws IllegalArgumentException if less than 6 elements, representing 3 points, are provided */
        public PolygonF (Vector2[] vertices)
        {
            Vertices = vertices;
        }

        /** Returns the polygon's local vertices without scaling or rotation and without being offset by the polygon position. */
        public Vector2[] Vertices
        {
            get { return _localVertices; }
            set
            {
                if (value.Length < 3)
                    throw new InvalidOperationException("polygons must contain at least 3 points.");

                _localVertices = value;
                _isDirty = true;
            }
        }

        /** Calculates and returns the vertices of the polygon after scaling, rotation, and positional translations have been applied,
         * as they are position within the world.
         * 
         * @return vertices scaled, rotated, and offset by the polygon position. */
        public Vector2[] GetTransformedVertices()
        {
            if (!_isDirty)
                return _worldVertices;

            _isDirty = false;

            var localVertices = _localVertices;

            if (_worldVertices == null || _worldVertices.Length != localVertices.Length)
                _worldVertices = new Vector2[localVertices.Length];

            var worldVertices = _worldVertices;
            var positionX = _position.X;
            var positionY = _position.Y;
            var originX = _origin.X;
            var originY = _origin.Y;
            var scaleX = _scale.X;
            var scaleY = _scale.Y;
            var isScaled = _scale != Vector2.One;
            var rotation = _rotation;
            var cos = (float)Math.Cos(rotation); // degrees?
            var sin = (float)Math.Sin(rotation);

            for (int i = 0, n = localVertices.Length; i < n; i += 2)
            {
                var x = localVertices[i].X - originX;
                var y = localVertices[i].Y - originY;

                // scale if needed
                if (isScaled)
                {
                    x *= scaleX;
                    y *= scaleY;
                }

                // rotate if needed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (rotation != 0)
                {
                    var oldX = x;
                    x = cos * x - sin * y;
                    y = sin * oldX + cos * y;
                }

                worldVertices[i].X = positionX + x + originX;
                worldVertices[i].Y = positionY + y + originY;
            }

            return worldVertices;
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                _isDirty = true;
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _isDirty = true;
            }
        }

        /** Returns the area contained within the polygon. */
        //public float area () {
        //    float[] vertices = getTransformedVertices();
        //    return GeometryUtils.polygonArea(vertices, 0, vertices.length);
        //}

        /** Returns an axis-aligned bounding box of this polygon.
         * 
         * Note the returned Rectangle is cached in this polygon, and will be reused if this Polygon is changed.
         * 
         * @return this polygon's bounding box {@link Rectangle} */
        //public Rectangle getBoundingRectangle () {
        //    float[] vertices = getTransformedVertices();

        //    float minX = vertices[0];
        //    float minY = vertices[1];
        //    float maxX = vertices[0];
        //    float maxY = vertices[1];

        //    final int numFloats = vertices.length;
        //    for (int i = 2; i < numFloats; i += 2)
        //    {
        //        minX = minX > vertices[i] ? vertices[i] : minX;
        //        minY = minY > vertices[i + 1] ? vertices[i + 1] : minY;
        //        maxX = maxX < vertices[i] ? vertices[i] : maxX;
        //        maxY = maxY < vertices[i + 1] ? vertices[i + 1] : maxY;
        //    }

        //    if (_bounds == null) _bounds = new Rectangle();
        //    _bounds.x = minX;
        //    _bounds.y = minY;
        //    _bounds.width = maxX - minX;
        //    _bounds.height = maxY - minY;

        //    return _bounds;
        //}

        /** Returns whether an x, y pair is contained within the polygon. */
    //    @Override
    //public boolean contains (float x, float y) {
    //        final float[] vertices = getTransformedVertices();
    //        final int numFloats = vertices.length;
    //        int intersects = 0;

    //        for (int i = 0; i < numFloats; i += 2)
    //        {
    //            float x1 = vertices[i];
    //            float y1 = vertices[i + 1];
    //            float x2 = vertices[(i + 2) % numFloats];
    //            float y2 = vertices[(i + 3) % numFloats];
    //            if (((y1 <= y && y < y2) || (y2 <= y && y < y1)) && x < ((x2 - x1) / (y2 - y1) * (y - y1) + x1)) intersects++;
    //        }
    //        return (intersects & 1) == 1;
    //    }

    //    @Override
    //public boolean contains (Vector2 point) {
    //        return contains(point.x, point.y);
    //    }
    }
}