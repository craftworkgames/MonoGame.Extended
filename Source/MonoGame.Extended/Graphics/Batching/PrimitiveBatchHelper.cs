using System;

namespace MonoGame.Extended.Graphics.Batching
{
    public static class PrimitiveBatchHelper
    {
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
