using MonoGame.Extended.Collision.BoundingVolumes;
using MonoGame.Extended.Collision.Shapes;

namespace MonoGame.Extended.Collision
{
    public sealed class Collider2D
    {
        internal int Index;
        internal ColliderFlags Flags = ColliderFlags.All;

        public BoundingVolume2D BoundingVolume { get; }
        public CollisionShape2D Shape { get; }
        public Transform2D Transform { get; }
        public object Owner { get; }

        internal Collider2D(object owner, Transform2D transform, CollisionShape2D shape, BoundingVolumeType2D boundingVolumeType)
        {
            Owner = owner;
            Transform = transform;
            Transform.BecameDirty += BecameDirty;
            BoundingVolume = BoundingVolume2D.Create(boundingVolumeType);
            Shape = shape;
            ShapeChanged();
            shape.ShapeChanged += ShapeChanged;
        }

        private void BecameDirty()
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

                BoundingVolume.UpdateFrom(Shape.LocalVertices);
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
