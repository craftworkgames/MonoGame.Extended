using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Tweening.Interpolators
{
    public class HslColorInterpolator : Interpolator<HslColor>
    {
        public override HslColor Add(HslColor t1, HslColor t2) => new HslColor(t1.H + t2.H, t1.S + t2.S, t1.L + t2.L);

        public override HslColor Mult(HslColor value, double amount)
            => new HslColor((float) (value.H*amount), (float) (value.S*amount), (float) (value.L*amount));

        public override HslColor Substract(HslColor t1, HslColor t2)
            => new HslColor(t1.H - t2.H, t1.S - t2.S, t1.L - t2.L);
    }
}