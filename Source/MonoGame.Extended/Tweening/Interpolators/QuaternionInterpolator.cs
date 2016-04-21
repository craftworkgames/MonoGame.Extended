using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class QuaternionInterpolator : Interpolator<Quaternion>
    {
        public override Quaternion Add(Quaternion t1, Quaternion t2) => t1 + t2;
        public override Quaternion Mult(Quaternion value, double amount) => value*(float) amount;
        public override Quaternion Substract(Quaternion t1, Quaternion t2) => t1 - t2;
    }
}