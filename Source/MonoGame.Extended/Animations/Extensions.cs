using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Animations
{
    internal static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T2> GetOrAddList<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key) {
            List<T2> list;
            if (!dictionary.TryGetValue(key, out list)) {
                list = new List<T2>();
                dictionary[key] = list;
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T2> ToEnumerable<T2>(this Dictionary<object, IEnumerable<T2>> dictionary) {
            return dictionary.Values.SelectMany(enumerable => enumerable);
        }
    }
}