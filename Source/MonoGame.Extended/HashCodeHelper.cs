using System;

namespace MonoGame.Extended
{
    public static class HashCodeHelper
    {
        // FNV
        private const uint Offset = 2166136261;
        private const int Prime = 16777619;

        public static unsafe int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            unchecked
            {
                var hash = Offset;
                byte* bytes = stackalloc byte[4];

                *(int*)bytes = arg1.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg2.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                return (int)hash;
            }
        }

        public static unsafe int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            unchecked
            {
                var hash = Offset;
                byte* bytes = stackalloc byte[4];

                *(int*)bytes = arg1.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg2.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg3.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                return (int)hash;
            }
        }

        public static unsafe int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            unchecked
            {
                var hash = Offset;
                byte* bytes = stackalloc byte[4];

                *(int*)bytes = arg1.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg2.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg3.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                *(int*)bytes = arg4.GetHashCode();
                hash = (hash * Prime) ^ bytes[0];
                hash = (hash * Prime) ^ bytes[1];
                hash = (hash * Prime) ^ bytes[2];
                hash = (hash * Prime) ^ bytes[3];

                return (int)hash;
            }
        }
    }
}
