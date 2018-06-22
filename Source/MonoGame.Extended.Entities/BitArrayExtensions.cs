using System;
using System.Collections;
using System.Collections.Specialized;

namespace MonoGame.Extended.Entities
{
    public static class BitVector32Extensions
    {
        public static bool IsEmpty(this BitVector32 bitVector)
        {
            return bitVector.Data == 0;
        }

    }

    public static class BitArrayExtensions
    {
        public static bool IsEmpty(this BitArray bitArray)
        {
            for (var i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return false;
            }

            return true;
        }

        public static bool ContainsAll(this BitArray bitArray, BitArray other)
        {
            var otherBitsLength = other.Length;
            var bitsLength = bitArray.Length;

            for (var i = bitsLength; i < otherBitsLength; i++)
            {
                if (other[i])
                    return false;
            }

            var s = Math.Min(bitsLength, otherBitsLength);

            for (var i = 0; s > i; i++)
            {
                if ((bitArray[i] & other[i]) != other[i])
                    return false;
            }

            return true;
        }

        public static bool Intersects(this BitArray bitArray, BitArray other)
        {
            var s = Math.Min(bitArray.Length, other.Length);

            for (var i = 0; s > i; i++)
            {
                if (bitArray[i] & other[i])
                    return true;
            }

            return false;
        }
    }
}