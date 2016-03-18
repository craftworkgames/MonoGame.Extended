using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class BoxFillProfile : Profile
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading)
        {
            offset = new Vector2(FastRand.NextSingle(Width * -0.5f, Width * 0.5f),
                                     FastRand.NextSingle(Height * -0.5f, Height * 0.5f));

            FastRand.NextUnitVector(out heading);
        }
    }
}