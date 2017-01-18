using System.Runtime.CompilerServices;

namespace MonoGame.Extended
{
    public static class FloatHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float value1, ref float value2)
        {
            var temp = value1;
            value1 = value2;
            value2 = temp;
        }
    }
}
