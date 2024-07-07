using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Profiles
{
    public abstract class Profile
    {
        public enum CircleRadiation
        {
            None,
            In,
            Out
        }

        protected FastRandom Random { get; } = new FastRandom();

        public abstract void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading);

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static Profile Point()
        {
            return new PointProfile();
        }

        public static Profile Line(Vector2 axis, float length)
        {
            return new LineProfile {Axis = axis, Length = length};
        }

        public static Profile Ring(float radius, CircleRadiation radiate)
        {
            return new RingProfile {Radius = radius, Radiate = radiate};
        }

        public static Profile Box(float width, float height)
        {
            return new BoxProfile {Width = width, Height = height};
        }

        public static Profile BoxFill(float width, float height)
        {
            return new BoxFillProfile {Width = width, Height = height};
        }

        public static Profile BoxUniform(float width, float height)
        {
            return new BoxUniformProfile {Width = width, Height = height};
        }

        public static Profile Circle(float radius, CircleRadiation radiate)
        {
            return new CircleProfile {Radius = radius, Radiate = radiate};
        }

        public static Profile Spray(Vector2 direction, float spread)
        {
            return new SprayProfile {Direction = direction, Spread = spread};
        }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }
}