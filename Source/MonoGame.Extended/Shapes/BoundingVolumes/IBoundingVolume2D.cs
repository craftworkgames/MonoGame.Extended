using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.BoundingVolumes
{
    public interface IBoundingVolume2D<TIntersectBoundingVolume> where TIntersectBoundingVolume : IBoundingVolume2D<TIntersectBoundingVolume>
    {
        bool Intersects(ref TIntersectBoundingVolume boundingVolume);
        void Compute(IReadOnlyList<Vector2> vertices);
    }
}
