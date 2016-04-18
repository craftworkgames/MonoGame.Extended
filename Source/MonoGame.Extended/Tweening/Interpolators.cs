using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Tweening
{
    public class DoubleInterpolator : Interpolator<double>
    {
        public override double Add(double t1, double t2) => t1 + t2;
        public override double Mult(double value, double amount) => value*amount;
        public override double Substract(double t1, double t2) => t1 - t2;
    }

    public class FloatInterpolator : Interpolator<float>
    {
        public override float Add(float t1, float t2) => t1 + t2;
        public override float Mult(float value, double amount) => (float) (value*amount);
        public override float Substract(float t1, float t2) => t1 - t2;
    }

    public class IntInterpolator : Interpolator<int>
    {
        public override int Add(int t1, int t2) => t1 + t2;
        public override int Mult(int value, double amount) => (int) (value*amount);
        public override int Substract(int t1, int t2) => t1 - t2;
    }

    public class Vector2Interpolator : Interpolator<Vector2>
    {
        public override Vector2 Add(Vector2 t1, Vector2 t2) => t1 + t2;
        public override Vector2 Mult(Vector2 value, double amount) => value*(float) amount;
        public override Vector2 Substract(Vector2 t1, Vector2 t2) => t1 - t2;
    }

    public class Vector3Interpolator : Interpolator<Vector3>
    {
        public override Vector3 Add(Vector3 t1, Vector3 t2) => t1 + t2;
        public override Vector3 Mult(Vector3 value, double amount) => value*(float) amount;
        public override Vector3 Substract(Vector3 t1, Vector3 t2) => t1 - t2;
    }

    public class Vector4Interpolator : Interpolator<Vector4>
    {
        public override Vector4 Add(Vector4 t1, Vector4 t2) => t1 + t2;
        public override Vector4 Mult(Vector4 value, double amount) => value*(float) amount;
        public override Vector4 Substract(Vector4 t1, Vector4 t2) => t1 - t2;
    }

    public class QuaternionInterpolator : Interpolator<Quaternion>
    {
        public override Quaternion Add(Quaternion t1, Quaternion t2) => t1 + t2;
        public override Quaternion Mult(Quaternion value, double amount) => value*(float) amount;
        public override Quaternion Substract(Quaternion t1, Quaternion t2) => t1 - t2;
    }

    public class ColorInterpolator : Interpolator<Color>
    {
        public override Color Add(Color t1, Color t2) => new Color(t1.R + t2.R, t1.G + t2.G, t1.B + t2.B, t1.A + t2.A);

        public override Color Mult(Color value, double amount)
            => new Color((byte) (value.R*amount), (byte) (value.G*amount), (byte) (value.B*amount),
                (byte) (value.A*amount));

        public override Color Substract(Color t1, Color t2)
            => new Color(t1.R - t2.R, t1.G - t2.G, t1.B - t2.B, t1.A - t2.A);
    }

    public class HslColorInterpolator : Interpolator<HslColor>
    {
        public override HslColor Add(HslColor t1, HslColor t2) => new HslColor(t1.H + t2.H, t1.S + t2.S, t1.L + t2.L);

        public override HslColor Mult(HslColor value, double amount)
            => new HslColor((float) (value.H*amount), (float) (value.S*amount), (float) (value.L*amount));

        public override HslColor Substract(HslColor t1, HslColor t2)
            => new HslColor(t1.H - t2.H, t1.S - t2.S, t1.L - t2.L);
    }
}