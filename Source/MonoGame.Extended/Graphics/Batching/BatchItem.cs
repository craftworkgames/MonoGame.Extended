using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchItem<TDrawContext>
        where TDrawContext : struct, IDrawContext<TDrawContext>
    {
        internal TDrawContext Context;
        internal readonly int StartIndex;
        internal int PrimitiveCount;
        internal byte PrimitiveType;

        internal BatchItem(PrimitiveType primitiveType, int startIndex, int primitiveCount, TDrawContext context)
        {
            PrimitiveType = (byte)primitiveType;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Context = context;
        }

        internal bool CanMergeIntoItem(ref TDrawContext otherContext, byte primitiveType)
        {
            return Context.Equals(ref otherContext) && PrimitiveType == primitiveType && (PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip || PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip);
        }
    }
}
