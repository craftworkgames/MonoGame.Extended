using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class Vector2Interpolator : Interpolator<Vector2>
    {
        public override Vector2 Add(Vector2 t1, Vector2 t2) => t1 + t2;
        public override Vector2 Mult(Vector2 value, double amount) => value*(float) amount;
        public override Vector2 Substract(Vector2 t1, Vector2 t2) => t1 - t2;
    }
}