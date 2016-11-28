using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public abstract class GraphicsShapeBuilder<TVertexType, TIndexType>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
    {
        public PrimitiveType FillPrimitiveType { get; }
        public PrimitiveType StrokePrimitiveType { get; }

        protected GraphicsShapeBuilder(PrimitiveType fillPrimitiveType, PrimitiveType strokePrimitiveType)
        {
            FillPrimitiveType = fillPrimitiveType;
            StrokePrimitiveType = strokePrimitiveType;
        }

        protected abstract int BuildVertices(TVertexType[] vertices, int startVertexIndex);

        protected abstract int BuildFillIndices(TIndexType[] indices, int startIndex, int indexOffset);

        public abstract int BuildStrokeIndices(TIndexType[] indices, int startIndex, int indexOffset);
    }
}
