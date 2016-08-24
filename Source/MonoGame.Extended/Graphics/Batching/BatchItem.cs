using System.Runtime.InteropServices;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchItem<TDrawContext>
        where TDrawContext : struct, IDrawContext
    {
        internal TDrawContext Context;
        internal readonly int StartIndex;
        internal readonly int PrimitiveCount;

        internal BatchItem(int startIndex, int primitiveCount, TDrawContext context)
        {
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Context = context;
        }
    }
}
