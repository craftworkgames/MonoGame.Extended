using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DrawCall<TDrawCallData> : IComparable<DrawCall<TDrawCallData>> where TDrawCallData : IDrawCallData
    {
        public ulong Key;
        public int BaseVertex;
        public int StartIndex;
        public int PrimitiveCount;
        public PrimitiveType PrimitiveType;
        public TDrawCallData Data;

        public int CompareTo(DrawCall<TDrawCallData> other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
