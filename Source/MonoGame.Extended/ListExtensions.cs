using System.Collections.Generic;

namespace MonoGame.Extended
{
    public static class ListExtensions
    {
        public static bool FastRemove<T>(this List<T> list, T item)
        {
            // O(n)
            var index = list.IndexOf(item);
            // O(1)
            return list.FastRemove(index);
        }

        public static bool FastRemove<T>(this List<T> list, int index)
        {
            if (index < 0)
            {
                return false;
            }

            var lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            // O(1) because removing the last item in the array
            list.RemoveAt(lastIndex);

            return true;
        }
    }
}