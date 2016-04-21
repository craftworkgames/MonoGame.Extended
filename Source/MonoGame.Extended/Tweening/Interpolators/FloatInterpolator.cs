namespace MonoGame.Extended.Tweening.Interpolators
{
    public class FloatInterpolator : Interpolator<float>
    {
        public override float Add(float t1, float t2) => t1 + t2;
        public override float Mult(float value, double amount) => (float) (value*amount);
        public override float Substract(float t1, float t2) => t1 - t2;
    }
}