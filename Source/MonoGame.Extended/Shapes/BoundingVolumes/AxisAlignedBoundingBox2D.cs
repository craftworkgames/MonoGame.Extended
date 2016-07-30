using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using TShapeVertexType = MonoGame.Extended.Shapes.Explicit.ShapeVertex2D;

namespace MonoGame.Extended.Shapes.BoundingVolumes
{
    /// <summary>
    ///     Defines a two-dimensional, rectangular four-sided box where the face normals are always parallel with the
    ///     coordinate system axes.
    /// </summary>
    /// <remarks>
    ///     <para>The name, 'Axis Aligned Bounding Box', is abbreviated as <i>AABB</i>.</para>
    ///     <para>
    ///         Axis-aligned bounding boxes are one of the most common bounding volumes due to its fast overlap check by direct
    ///         comparison of individual coordinate values.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay(value: "{DebugDisplayString,nq}")]
    public struct AxisAlignedBoundingBox2D : IEquatable<AxisAlignedBoundingBox2D>, IBoundingVolume<TShapeVertexType, AxisAlignedBoundingBox2D>
    {
        public Vector2 Center;
        public SizeF HalfSize;

        public AxisAlignedBoundingBox2D(Vector2 center, SizeF halfSize)
        {
            Center = center;
            HalfSize = halfSize;
        }

        public bool Intersects(ref AxisAlignedBoundingBox2D other)
        {
            var positionVector = Center - other.Center;
            var totalHalfExtents = HalfSize + other.HalfSize;
            return Math.Abs(positionVector.X) > totalHalfExtents.Width || Math.Abs(positionVector.Y) > totalHalfExtents.Height;
        }

        public void UpdateFrom(IReadOnlyList<TShapeVertexType> vertices)
        {
            var minimum = new Vector2(float.MaxValue, float.MaxValue);
            var maximum = new Vector2(float.MinValue, float.MinValue);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < vertices.Count; ++index)
            {
                var vertex = vertices[index];
                Vector2.Min(ref minimum, ref vertex.Position, out minimum);
                Vector2.Max(ref maximum, ref vertex.Position, out maximum);
            }

            Center = (maximum + minimum) * 0.5f;
            HalfSize = (maximum - minimum) * 0.5f;
        }

        public static bool operator ==(AxisAlignedBoundingBox2D first, AxisAlignedBoundingBox2D second)
        {
            return first.Center == second.Center && first.HalfSize == second.HalfSize;
        }

        public static bool operator !=(AxisAlignedBoundingBox2D first, AxisAlignedBoundingBox2D second)
        {
            return !(first == second);
        }

        public bool Equals(AxisAlignedBoundingBox2D other)
        {
            return Center == other.Center && HalfSize == other.HalfSize;
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
            get { return $"Center = {Center}, Extents = {HalfSize}"; }
        }

        public override string ToString()
        {
            return $"{{Center = {Center}, Extents = {HalfSize}}}";
        }
    }
}
