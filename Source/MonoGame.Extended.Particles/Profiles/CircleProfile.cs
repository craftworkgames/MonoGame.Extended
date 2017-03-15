using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class CircleProfile : Profile
    {
        public float Radius { get; set; }
        public CircleRadiation Radiate { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading)
        {
            var dist = Random.NextSingle(0f, Radius);

            Random.NextUnitVector(out heading);

            offset = Radiate == CircleRadiation.In
                ? new Vector2(-heading.X*dist, -heading.Y*dist)
                : new Vector2(heading.X*dist, heading.Y*dist);

            if (Radiate == CircleRadiation.None)
                Random.NextUnitVector(out heading);
        }
    }
}