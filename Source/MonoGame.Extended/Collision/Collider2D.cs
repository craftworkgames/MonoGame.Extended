using System;
using MonoGame.Extended.Collision.Broadphase.BoundingVolumes;
using MonoGame.Extended.Collision.Narrowphase.Shapes;

namespace MonoGame.Extended.Collision
{
    [Flags]
    internal enum ColliderFlags : byte
    {
        ShapeIsDirty = 1 << 0,
        WorldBoundingVolumeIsDirty = 1 << 1,
        All = ShapeIsDirty | WorldBoundingVolumeIsDirty
    }

    public sealed class Collider2D
    {
        internal int Index;
        internal ColliderFlags Flags = ColliderFlags.All;

        public BoundingVolume2D BoundingVolume { get; }
        public CollisionShape2D Shape { get; }
        public ITransform2D Transform { get; }

        internal Collider2D(ITransform2D transform, CollisionShape2D shape, BoundingVolumeType2D boundingVolumeType)
        {
            Transform = transform;
            Transform.TransformBecameDirty += TransformBecameDirty;
            BoundingVolume = BoundingVolume2D.Create(boundingVolumeType);
            Shape = shape;
            ShapeChanged();
            shape.ShapeChanged += ShapeChanged;
            Transform = transform;
        }

        private void TransformBecameDirty()
        {
            Flags |= ColliderFlags.WorldBoundingVolumeIsDirty;
        }

        private void ShapeChanged()
        {
            Flags |= ColliderFlags.ShapeIsDirty | ColliderFlags.WorldBoundingVolumeIsDirty;
        }

        internal void UpdateIfNecessary(BoundingVolume2D worldBoundingVolume)
        {
            // ReSharper disable once InvertIf
            if ((Flags & ColliderFlags.ShapeIsDirty) != 0)
            {
                BoundingVolume.UpdateFrom(Shape.Points);
                Flags &= ~ColliderFlags.ShapeIsDirty;
            }

            if ((Flags & ColliderFlags.WorldBoundingVolumeIsDirty) != 0)
            {
                Matrix2D matrix;
                Transform.GetWorldMatrix(out matrix);
                worldBoundingVolume.UpdateFrom(BoundingVolume, ref matrix);
                Flags &= ~ColliderFlags.WorldBoundingVolumeIsDirty;
            }
        }
    }
}
