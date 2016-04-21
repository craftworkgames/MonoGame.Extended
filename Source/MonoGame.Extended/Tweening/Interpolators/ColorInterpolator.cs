using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class ColorInterpolator : Interpolator<Color>
    {
        public override Color Add(Color t1, Color t2) => new Color(t1.R + t2.R, t1.G + t2.G, t1.B + t2.B, t1.A + t2.A);

        public override Color Mult(Color value, double amount)
            => new Color((byte) (value.R*amount), (byte) (value.G*amount), (byte) (value.B*amount),
                (byte) (value.A*amount));

        public override Color Substract(Color t1, Color t2)
            => new Color(t1.R - t2.R, t1.G - t2.G, t1.B - t2.B, t1.A - t2.A);
    }
}