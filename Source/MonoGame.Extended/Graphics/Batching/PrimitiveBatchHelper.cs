using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchHelper
    {
        internal static readonly int[] QuadrilateralClockwiseIndices;

        static PrimitiveBatchHelper()
        {
            /*
                     *  TL    TR
                     *   0----1 Vertex Indices: { 0, 1, 2, 3 }
                     *   |   /| Triangle 1: 2-0-1 (Clockwise)
                     *   |  / | Triangle 2: 1-3-2 (Clockwise)
                     *   | /  |
                     *   |/   |
                     *   2----3
                     *  BL    BR
                     */
            QuadrilateralClockwiseIndices = new[]
            {
                0,
                1,
                2,
                1,
                3,
                2
            };
        }

        public static Action<Array, Array, int, int> SortAction { get; set; }

        internal static void SetQuadrilateralRectangleVertexPositionsFromTopLeft(ref Vector2 topLeftPosition, ref SizeF size, float rotation, float depth, ref Vector2 origin, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomLeft, out Vector2 bottomRight)
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

            topRight.X = x + (dx + w) * cos - dy * sin;
            topRight.Y = y + (dx + w) * sin + dy * cos;

            bottomLeft.X = x + dx * cos - (dy + h) * sin;
            bottomLeft.Y = y + dx * sin + (dy + h) * cos;

            bottomRight.X = x + (dx + w) * cos - (dy + h) * sin;
            bottomRight.Y = y + (dx + w) * sin + (dy + h) * cos;
        }

        internal static void SetQuadrilateralRectangleVertexPositionsFromCenter(ref Vector2 centerPosition, ref SizeF halfSize, float depth, float rotation, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomLeft, out Vector2 bottomRight)
        {
            var halfWidthCos = halfSize.Width * (float)Math.Cos(rotation);
            var halfHeightSin = halfSize.Height * (float)Math.Sin(rotation);

            topLeft.X = centerPosition.X - halfWidthCos;
            topLeft.Y = centerPosition.Y - halfHeightSin;

            topRight.X = centerPosition.X + halfWidthCos;
            topRight.Y = centerPosition.Y - halfHeightSin;

            bottomLeft.X = centerPosition.X - halfWidthCos;
            bottomLeft.Y = centerPosition.Y + halfHeightSin;

            bottomRight.X = centerPosition.X + halfWidthCos;
            bottomRight.Y = centerPosition.Y + halfHeightSin;
        }

        internal static void SetQuadrilateralRectangleVertexPositionsFromCenter(ref Vector2 centerPosition, ref SizeF halfSize, float depth, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomLeft, out Vector2 bottomRight)
        {
            topLeft.X = centerPosition.X - halfSize.Width;
            topLeft.Y = centerPosition.Y - halfSize.Height;

            topRight.X = centerPosition.X + halfSize.Width;
            topRight.Y = centerPosition.Y - halfSize.Height;

            bottomLeft.X = centerPosition.X - halfSize.Width;
            bottomLeft.Y = centerPosition.Y + halfSize.Height;

            bottomRight.X = centerPosition.X + halfSize.Width;
            bottomRight.Y = centerPosition.Y + halfSize.Height;
        }
    }
}
