using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Particles {
    /// <summary>
    /// Defines a part of a line that is bounded by two distinct end points.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LineSegment : IEquatable<LineSegment> {
        internal Vector _point1;
        internal Vector _point2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> structure.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public LineSegment(Vector point1, Vector point2) {
            _point1 = point1;
            _point2 = point2;
        }

        public static LineSegment FromPoints(Vector p1, Vector p2)
            => new LineSegment(p1, p2);

        public static LineSegment FromOrigin(Vector origin, Vector vector)
            => new LineSegment(origin, origin + vector);

        public Vector Origin => _point1;

        public Axis Direction {
            get {
                var coord = _point2 - _point1;
                return new Axis(coord.X, coord.Y);
            }
        }

        public void Translate(Vector t)
        {
            _point1 += t;
            _point2 += t;
        }

        public Vector ToVector() {
            var t = _point2 - _point1;
            return new Vector(t.X, t.Y);
        }

        public unsafe void CopyTo(float* destination) {
            _point1.CopyTo(destination);
            _point2.CopyTo(destination + sizeof(Vector));
        }

        public void Destructure(out Vector point1, out Vector point2) {
            point1 = _point1;
            point2 = _point2;
        }

        public void Match(Action<Vector, Vector> callback) {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
        }

        public T Map<T>(Func<Vector, Vector, T> map) {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(_point1, _point2);
        }

        public bool Equals(LineSegment other) {
            return _point1.Equals(other._point1) &&
                   _point2.Equals(other._point2);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is LineSegment & Equals((LineSegment)obj);
        }

        public override int GetHashCode() {
            var hashCode = _point1.GetHashCode();

            hashCode = (hashCode * 397) ^ _point2.GetHashCode();

            return hashCode;
        }

        public override string ToString() {
            return string.Format("({0:x}:{0:y},{1:x}:{1:y})", _point1, _point2);
        }
    }
}