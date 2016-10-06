using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.BoundingVolumes
{
    public abstract class BoundingVolume2D
    {
        public static BoundingVolume2D Create(BoundingVolumeType2D type)
        {
            BoundingVolume2D boundingVolume;

            switch (type)
            {
                case BoundingVolumeType2D.BoundingBox:
                    boundingVolume = new BoundingBox2D();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return boundingVolume;
        }

        public BoundingVolumeType2D Type { get; }

        protected BoundingVolume2D(BoundingVolumeType2D type)
        {
            Type = type;
        }

        public abstract bool Intersects(Ray ray, out float rayNearDistance, out float rayFarDistance);
        public abstract bool Intersects(BoundingVolume2D boundingVolume);
        public abstract bool Contains(Vector2 point);
        public abstract void UpdateFrom(IReadOnlyList<Vector2> vertices);
        public abstract void UpdateFrom(BoundingVolume2D boundingVolume, ref Matrix2D localToWorldMatrix);
    }
}
