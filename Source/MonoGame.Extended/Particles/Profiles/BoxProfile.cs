using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public class BoxProfile : Profile
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public override void GetOffsetAndHeading(out Vector2 offset, out Axis heading)
        {
            switch (FastRand.NextInteger(3))
            {
                case 0: // Left
                    offset = new Vector2(Width*-0.5f, FastRand.NextSingle(Height*-0.5f, Height*0.5f));
                    break;
                case 1: // Top
                    offset = new Vector2(FastRand.NextSingle(Width*-0.5f, Width*0.5f), Height*-0.5f);
                    break;
                case 2: // Right
                    offset = new Vector2(Width*0.5f, FastRand.NextSingle(Height*-0.5f, Height*0.5f));
                    break;
                default: // Bottom
                    offset = new Vector2(FastRand.NextSingle(Width*-0.5f, Width*0.5f), Height*0.5f);
                    break;
            }

            FastRand.NextUnitVector(out heading);
        }
    }
}