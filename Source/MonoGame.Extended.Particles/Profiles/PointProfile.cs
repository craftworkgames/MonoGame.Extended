using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class PointProfile : Profile
    {
        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading)
        {
            offset = Vector2.Zero;

            Random.NextUnitVector(out heading);
        }
    }
}