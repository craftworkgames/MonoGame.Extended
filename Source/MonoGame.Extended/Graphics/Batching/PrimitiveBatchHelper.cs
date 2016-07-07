using System;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchHelper
    {
        /*
         *  TL    TR
         *   0----1 Vertex Indices: { 0, 1, 2, 3 }
         *   |   /| Triangle 1: 0-1-2 (Clockwise)
         *   |  / | Triangle 2: 1-3-2 (Clockwise)
         *   | /  |
         *   |/   |
         *   2----3
         *  BL    BR
         * Note: The order of the vertex indices for the first triangle is important (0-1-2).
         *      It because indices for lines and triangles use this array with the first 2 to 3 vertex indices.
         *      The order of vertex indices for the second triangle is not as important aslong as they have clockwise winding. 
         */
        internal static readonly int[] QuadrilateralClockwiseIndices = {
            0,
            1,
            2,
            1,
            3,
            2
        };

        public static Action<Array, Array, int, int> SortAction { get; set; }
    }
}
