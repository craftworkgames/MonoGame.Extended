using System.Collections.Generic;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Shapes.BoundingVolumes
{
    public interface IBoundingVolume<in TShapeVertexType, TIntersectBoundingVolume>
        where TShapeVertexType : struct, IShapeVertexType<TShapeVertexType>
        where TIntersectBoundingVolume : IBoundingVolume<TShapeVertexType, TIntersectBoundingVolume>
    {
        bool Intersects(ref TIntersectBoundingVolume boundingVolume);
        void UpdateFrom(IReadOnlyList<TShapeVertexType> vertices);
    }
}
