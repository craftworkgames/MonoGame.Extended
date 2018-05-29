using Microsoft.Xna.Framework;

namespace Platformer.Collisions
{
    public struct Manifold
    {
        public Body BodyA;
        public Body BodyB;
        public float Penetration;
        public Vector2 Normal;
        public Vector2 Overlap;
    }
}