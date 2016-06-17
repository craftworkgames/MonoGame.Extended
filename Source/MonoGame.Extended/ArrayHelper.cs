using System;

namespace MonoGame.Extended
{
    public static class ArrayHelper
    {
        internal const int MaxArrayLength = 2146435071;
        internal const float GoldenRatio = 1.61803398875f;

        public static void AddItemDynamicallyIfNecessary<T>(ref T[] array, T item, ref int itemsCount)
        {
            if (itemsCount >= array.Length)
            {
                ReSizeIfNecessary(ref array, itemsCount + 1);
            }
            array[itemsCount++] = item;
        }

        public static void ReSizeIfNecessary<T>(ref T[] array, int minimumArrayLength)
        {
            if (array.Length >= minimumArrayLength)
            {
                return;
            }

            var newArrayLength = (int)(array.Length * GoldenRatio);
            if ((uint)newArrayLength > MaxArrayLength)
            {
                newArrayLength = MaxArrayLength;
            }
            else if (newArrayLength < minimumArrayLength)
            {
                newArrayLength = minimumArrayLength;
            }

            if (newArrayLength != array.Length)
            {
                Array.Resize(ref array, newArrayLength);
            }
        }

        public static bool RemoveItemAtIndexBySwappingToBack<T>(ref T[] array, int indexToRemove, ref int itemsCount)
        {
            if (indexToRemove < 0 || indexToRemove > itemsCount)
            {
                return false;
            }

            array[indexToRemove] = array[itemsCount];
            array[itemsCount--] = default(T);
            return true;
        }
    }
}
