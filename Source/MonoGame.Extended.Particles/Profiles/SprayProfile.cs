using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class SprayProfile : Profile
    {
        public Vector2 Direction { get; set; }
        public float Spread { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading)
        {
            var angle = (float) Math.Atan2(Direction.Y, Direction.X);

            angle = Random.NextSingle(angle - Spread/2f, angle + Spread/2f);
            offset = Vector2.Zero;
            heading = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
        }
    }
}