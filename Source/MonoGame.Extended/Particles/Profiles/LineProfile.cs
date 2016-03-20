using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class LineProfile : Profile
    {
        public Axis Axis { get; set; }
        public float Length { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading)
        {
            var vect = Axis * FastRand.NextSingle(Length * -0.5f, Length * 0.5f);
            offset = new Vector2(vect.X, vect.Y);
            FastRand.NextUnitVector(out heading);
        }
    }
}