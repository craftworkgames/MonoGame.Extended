using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchHelper
    {
        internal static readonly int[] QuadIndices;

        static PrimitiveBatchHelper()
        {
            QuadIndices = new[]
            {
                0,
                2,
                3,
                0,
                3,
                1
            };
        }

        public static Action<Array, Array, int, int> SortAction { get; set; }

        internal static void SetQuadrilateralRectangleVertexPositionsFromTopLeft(ref Vector2 topLeftPosition, ref SizeF size, float rotation, float depth, ref Vector2 origin, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
        {
            var x = topLeftPosition.X;
            var y = topLeftPosition.Y;
            var w = size.Width;
            var h = size.Height;
            var dx = -origin.X;
            var dy = -origin.Y;
            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);

            topLeft.X = x + dx * cos - dy * sin;
            topLeft.Y = y + dx * sin + dy * cos;
            topLeft.Z = depth;

            topRight.X = x + (dx + w) * cos - dy * sin;
            topRight.Y = y + (dx + w) * sin + dy * cos;
            topRight.Z = depth;

            bottomLeft.X = x + dx * cos - (dy + h) * sin;
            bottomLeft.Y = y + dx * sin + (dy + h) * cos;
            bottomLeft.Z = depth;

            bottomRight.X = x + (dx + w) * cos - (dy + h) * sin;
            bottomRight.Y = y + (dx + w) * sin + (dy + h) * cos;
            bottomRight.Z = depth;
        }

        internal static void SetQuadrilateralRectangleVertexPositionsFromCenter(ref Vector2 centerPosition, ref SizeF halfSize, float depth, float rotation, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
        {
            var halfWidthCos = halfSize.Width * (float)Math.Cos(rotation);
            var halfHeightSin = halfSize.Height * (float)Math.Sin(rotation);

            topLeft.X = centerPosition.X - halfWidthCos;
            topLeft.Y = centerPosition.Y - halfHeightSin;
            topLeft.Z = depth;

            topRight.X = centerPosition.X + halfWidthCos;
            topRight.Y = centerPosition.Y - halfHeightSin;
            topRight.Z = depth; 

            bottomLeft.X = centerPosition.X - halfWidthCos;
            bottomLeft.Y = centerPosition.Y + halfHeightSin;
            bottomLeft.Z = depth;

            bottomRight.X = centerPosition.X + halfWidthCos;
            bottomRight.Y = centerPosition.Y + halfHeightSin;
            bottomRight.Z = depth;
        }

        internal static void SetQuadrilateralRectangleVertexPositionsFromCenter(ref Vector2 centerPosition, ref SizeF halfSize, float depth, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
        {
            topLeft.X = centerPosition.X - halfSize.Width;
            topLeft.Y = centerPosition.Y - halfSize.Height;
            topLeft.Z = depth;

            topRight.X = centerPosition.X + halfSize.Width;
            topRight.Y = centerPosition.Y - halfSize.Height;
            topRight.Z = depth;

            bottomLeft.X = centerPosition.X - halfSize.Width;
            bottomLeft.Y = centerPosition.Y + halfSize.Height;
            bottomLeft.Z = depth;

            bottomRight.X = centerPosition.X + halfSize.Width;
            bottomRight.Y = centerPosition.Y + halfSize.Height;
            bottomRight.Z = depth;
        }
    }
}
