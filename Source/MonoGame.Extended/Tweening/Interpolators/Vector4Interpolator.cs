using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class Vector4Interpolator : Interpolator<Vector4>
    {
        public override Vector4 Add(Vector4 t1, Vector4 t2) => t1 + t2;
        public override Vector4 Mult(Vector4 value, double amount) => value*(float) amount;
        public override Vector4 Substract(Vector4 t1, Vector4 t2) => t1 - t2;
    }
}