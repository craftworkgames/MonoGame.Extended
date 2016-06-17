using System.Collections.Generic;

namespace MonoGame.Extended
{
    public static class ListExtensions
    { 
        public static bool FastRemove<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
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
            list.RemoveAt(lastIndex);

            return true;
        }
    }
}
