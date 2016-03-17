using System;

namespace MonoGame.Extended.Particles.Profiles
{
    public class SprayProfile : Profile
    {
        public Axis Direction { get; set; }
        public float Spread { get; set; }

        public override void GetOffsetAndHeading(out Vector offset, out Axis heading) {
            var angle = Direction.Map((x, y) => (float)Math.Atan2(y, x));

            angle = FastRand.NextSingle(angle - Spread / 2f, angle + Spread / 2f);

            offset = Vector.Zero;
            heading = new Axis((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}