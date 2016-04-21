using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class Vector3Interpolator : Interpolator<Vector3>
    {
        public override Vector3 Add(Vector3 t1, Vector3 t2) => t1 + t2;
        public override Vector3 Mult(Vector3 value, double amount) => value*(float) amount;
        public override Vector3 Substract(Vector3 t1, Vector3 t2) => t1 - t2;
    }
}