using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.BoundingVolumes
{
    /// <summary>
    ///     Defines a two-dimensional, rectangular four-sided box where the face normals are always parallel with the
    ///     coordinate system axes.
    /// </summary>
    /// <remarks>
    ///     <para>The name, Axis Aligned Bounding Box, is abbreviated as <i>AABB</i>.</para>
    ///     <para>
    ///         Axis-aligned bounding boxes are one of the most common bounding volumes due to its fast overlap check by direct
    ///         comparison of individual coordinate values.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct AxisAlignedBoundingBox2D : IEquatable<AxisAlignedBoundingBox2D>, IBoundingVolume<AxisAlignedBoundingBox2D>
    {
        public Vector2 Center;
        public Vector2 HalfExtents;

        public AxisAlignedBoundingBox2D(Vector2 center, Vector2 halfExtents)
        {
            Center = center;
            HalfExtents = halfExtents;
        }

        public bool Intersects(ref AxisAlignedBoundingBox2D other)
        {
            var positionVector = Center - other.Center;
            var totalHalfExtents = HalfExtents + other.HalfExtents;
            return Math.Abs(positionVector.X) > totalHalfExtents.X || Math.Abs(positionVector.Y) > totalHalfExtents.Y;
        }

        public static bool operator ==(AxisAlignedBoundingBox2D first, AxisAlignedBoundingBox2D second)
        {
            return first.Center == second.Center && first.HalfExtents == second.HalfExtents;
        }

        public static bool operator !=(AxisAlignedBoundingBox2D first, AxisAlignedBoundingBox2D second)
        {
            return !(first == second);
        }

        public bool Equals(AxisAlignedBoundingBox2D other)
        {
            return Center == other.Center && HalfExtents == other.HalfExtents;
        }

        public override bool Equals(object obj)
        {
            return obj is AxisAlignedBoundingBox2D && Equals((AxisAlignedBoundingBox2D)obj);
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        internal string DebugDisplayString
        {
            get { return $"Center = {Center}, Extents = {HalfExtents}"; }
        }

        public override string ToString()
        {
            return $"{{Center = {Center}, Extents = {HalfExtents}}}";
        }
    }
}
