namespace MonoGame.Extended.Tweening.Interpolators
{
    public class IntInterpolator : Interpolator<int>
    {
        public override int Add(int t1, int t2) => t1 + t2;
        public override int Mult(int value, double amount) => (int) (value*amount);
        public override int Substract(int t1, int t2) => t1 - t2;
    }
}