namespace MonoGame.Extended.Shapes.BoundingVolumes
{
    public interface IBoundingVolume<TIntersectBoundingVolume> where TIntersectBoundingVolume : IBoundingVolume<TIntersectBoundingVolume>
    {
        bool Intersects(ref TIntersectBoundingVolume boundingVolume);
    }
}
