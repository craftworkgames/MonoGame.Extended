namespace MonoGame.Extended.Tweening.Interpolators
{
    public class DoubleInterpolator : Interpolator<double>
    {
        public override double Add(double t1, double t2) => t1 + t2;
        public override double Mult(double value, double amount) => value*amount;
        public override double Substract(double t1, double t2) => t1 - t2;
    }
}