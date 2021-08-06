using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class RingProfile : Profile
    {
        public float Radius { get; set; }
        public CircleRadiation Radiate { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading)
        {
            Random.NextUnitVector(out heading);

            switch (Radiate)
            {
                case CircleRadiation.In:
                    offset = new Vector2(-heading.X*Radius, -heading.Y*Radius);
                    break;
                case CircleRadiation.Out:
                    offset = new Vector2(heading.X*Radius, heading.Y*Radius);
                    break;
                case CircleRadiation.None:
                    offset = new Vector2(heading.X*Radius, heading.Y*Radius);
                    Random.NextUnitVector(out heading);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{Radiate} is not supported");
            }
        }
    }
}