using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Interpolation
{
    public class DoubleInterpolator : Interpolator<double>
    {
        public override double Add(double t1, double t2) => t1 + t2;
        public override double Mult(double value, double amount) => value * amount;
        public override double Substract(double t1, double t2) => t1 - t2;
    }
    public class FloatInterpolator : Interpolator<float>
    {
        public override float Add(float t1, float t2) => t1 + t2;
        public override float Mult(float value, double amount) => (float)(value * amount);
        public override float Substract(float t1, float t2) => t1 - t2;
    }
    public class Vector2Interpolator : Interpolator<Vector2>
    {
        public override Vector2 Add(Vector2 t1, Vector2 t2) => t1 + t2;
        public override Vector2 Mult(Vector2 value, double amount) => value * (float)amount;
        public override Vector2 Substract(Vector2 t1, Vector2 t2) => t1 - t2;
    }
    public class Vector3Interpolator : Interpolator<Vector3>
    {
        public override Vector3 Add(Vector3 t1, Vector3 t2) => t1 + t2;
        public override Vector3 Mult(Vector3 value, double amount) => value * (float)amount;
        public override Vector3 Substract(Vector3 t1, Vector3 t2) => t1 - t2;
    }
    public class Vector4Interpolator : Interpolator<Vector4>
    {
        public override Vector4 Add(Vector4 t1, Vector4 t2) => t1 + t2;
        public override Vector4 Mult(Vector4 value, double amount) => value * (float)amount;
        public override Vector4 Substract(Vector4 t1, Vector4 t2) => t1 - t2;
    }
    public class QuaternionInterpolator : Interpolator<Quaternion>
    {
        public override Quaternion Add(Quaternion t1, Quaternion t2) => t1 + t2;
        public override Quaternion Mult(Quaternion value, double amount) => value*(float) amount;
        public override Quaternion Substract(Quaternion t1, Quaternion t2) => t1 - t2;
    }
    //public class HslColorInterpolator : Interpolator<HslColor>
    //{
    //    public override HslColor Add(HslColor t1, HslColor t2) => t1 + t2;
    //    public override HslColor Mult(HslColor value, double amount) => value*(float) amount;
    //    public override HslColor Substract(HslColor t1, HslColor t2) => t1 - t2;
    //}
}