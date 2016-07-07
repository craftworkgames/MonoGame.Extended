using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static partial class RenderGeometryBuilder
    {
        public delegate void VertexDelegate<TVertexType>(ref TVertexType vertex) where TVertexType : struct, IVertexType;

        public delegate void VertexIndexDelegate(int index);

        // to use delegates without creating unecessary memory garbage, we need to "cache" the delegates
        private static VertexIndexDelegate _outputVertexIndex;
    }
}
