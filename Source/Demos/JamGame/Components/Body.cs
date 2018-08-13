using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace JamGame.Components
{
    public class Body
    {
        public Vector2 Velocity { get; set; }
        public Size2 Size { get; set; }
        public bool IsHit { get; set; }
    }
}