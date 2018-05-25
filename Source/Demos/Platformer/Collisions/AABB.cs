using Microsoft.Xna.Framework;

namespace Platformer.Collisions
{
    // ReSharper disable once InconsistentNaming
    public struct AABB
    {
        public AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Vector2 Min;
        public Vector2 Max;
        public Vector2 Center => new Vector2(CenterX, CenterY);
        public float CenterX => (Max.X - Min.X) / 2f;
        public float CenterY => (Max.Y - Min.Y) / 2f;
        public float Width => Max.X - Min.X;
        public float Height => Max.Y - Min.Y;
    }
}