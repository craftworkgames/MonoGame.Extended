/*

Orignal code from: https://referencesource.microsoft.com/#q=BitArray
Renamed to BitVector to avoid name clash
Modified for performance; removed checks and modified bitwise operations

https://github.com/Microsoft/referencesource/blob/master/LICENSE.txt

The MIT License (MIT)

Copyright (c) Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.

*/

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*=============================================================================
**
** Class: BitVector
**
** <OWNER>Microsoft</OWNER>
**
**
** Purpose: The BitVector class manages a compact array of bit values.
**
**
=============================================================================*/

using System;

namespace MonoGame.Extended.Entities
{
    internal sealed class BitVector
    {
        // XPerY=n means that n Xs can be stored in 1 Y. 
        private const int BitsPerInt32 = 32;

        private readonly int[] _array;
        private int _intLength;

        public int Length { get; }

        private BitVector()
        {
        }

        public BitVector(int length)
            : this(length, false)
        {
        }

        public BitVector(int length, bool defaultValue)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _array = new int[GetArrayLength(length, BitsPerInt32)];
            Length = length;

            var fillValue = defaultValue ? unchecked((int)0xffffffff) : 0;
            for (var i = 0; i < _array.Length; i++)
                _array[i] = fillValue;

            _intLength = GetArrayLength(Length, BitsPerInt32);
        }

        public BitVector(BitVector bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            var arrayLength = GetArrayLength(bits.Length, BitsPerInt32);
            _array = new int[arrayLength];
            Length = bits.Length;
            _intLength = GetArrayLength(Length, BitsPerInt32);

            Array.Copy(bits._array, _array, arrayLength);
        }

        public bool this[int index]
        {
            get
            {
                return (_array[index / 32] & (1 << (index % 32))) != 0;
            }
            set
            {
                if (value)
                    _array[index / 32] |= 1 << (index % 32);
                else
                    _array[index / 32] &= ~(1 << (index % 32));
            }
        }

        public void SetAll(bool value)
        {
            var fillValue = value ? unchecked((int)0xffffffff) : 0;
            for (var i = 0; i < _intLength; ++i)
                _array[i] = fillValue;
        }

        public void And(BitVector value, ref BitVector result)
        {
            for (var i = 0; i < _intLength; ++i)
                result._array[i] = _array[i] & value._array[i];
        }

        public bool Equals(BitVector value)
        {
            for (var i = 0; i < _intLength; ++i)
                if (_array[i] != value._array[i])
                    return false;

            return true;
        }

        public bool EqualsZero()
        {
            for (var i = 0; i < _intLength; ++i)
                if (_array[i] != 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Used for conversion between different representations of bit array. 
        /// Returns (n+(div-1))/div, rearranged to avoid arithmetic overflow. 
        /// For example, in the bit to int case, the straightforward calc would 
        /// be (n+31)/32, but that would cause overflow. So instead it's 
        /// rearranged to ((n-1)/32) + 1, with special casing for 0.
        /// 
        /// Usage:
        /// GetArrayLength(77, BitsPerInt32): returns how many ints must be 
        /// allocated to store 77 bits.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="div">use a conversion constant, e.g. BytesPerInt32 to get
        /// how many ints are required to store n bytes</param>
        /// <returns></returns>
        private static int GetArrayLength(int n, int div)
        {
            return n > 0 ? (n - 1) / div + 1 : 0;
        }

        public override string ToString()
        {
            string result = null;
            for (var i = 0; i < Length; ++i)
            {
                result += this[i] ? 1 : 0;
            }

            return result;
        }
    }

}