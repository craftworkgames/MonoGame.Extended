using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Particles.Profiles
{
    public abstract class Profile //: ICloneable
    {
        public abstract void GetOffsetAndHeading(out Vector2 offset, out Vector2 heading);

        public object Clone() {
            return MemberwiseClone();
        }

        public enum CircleRadiation { None, In, Out }

        public static Profile Point(PointF point) {
            return new OutlineProfile(point);
        }

        public static Profile Line(LineF line) {
            return new OutlineProfile(line);
        }

        public static Profile Ring(CircleF circle, CircleRadiation radiate) {
            return new OutlineProfile(circle, radiate, circle.Center);
        }

        public static Profile Box(RectangleF rectangle) {
            return new OutlineProfile(rectangle);
        }

        public static Profile BoxFill(RectangleF rectangle) {
            return new FillProfile(rectangle);
        }


        public static Profile Circle(CircleF circle, CircleRadiation radiate = CircleRadiation.None) {
            return new FillProfile(circle);
        }

        public static Profile Spray(IShapeF spawn, IShapeF target) {
            return new FillTargetProfile(spawn, target);
        }

        public override string ToString() {
            return GetType().ToString();
        }
    }
}
