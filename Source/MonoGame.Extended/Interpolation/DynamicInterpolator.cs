using System;

namespace MonoGame.Extended.Interpolation
{
    public class DynamicInterpolator : Interpolator<object>
    {
        private DynamicInterpolator() {}

        public static DynamicInterpolator Singleton = new DynamicInterpolator();

        public override dynamic Add(dynamic t1, dynamic t2) => t1 + t2;
        public override dynamic Mult(dynamic value, double amount) {
            try {
                value *= amount;
            }
            catch {
                value *= (float)amount;
            }
            return value;
        }

        public override dynamic Substract(dynamic t1, dynamic t2) => t1 - t2;
    }
}