using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class BounceEasing : EasingFunction
    {
        //TODO make bounces 
        public int Bounces { get; set; }


        private const double BOUNCE_CONST1 = 2.75;
        private static readonly double BOUNCE_CONST2 = Math.Pow(BOUNCE_CONST1, 2);
        protected override double Function(double t) {
            t = 1 - t; //flip x-axis
            if (t < (1 / BOUNCE_CONST1)) // big bounce
                return 1 - (float)(BOUNCE_CONST2 * t * t);
            if (t < (2 / BOUNCE_CONST1))
                return 1 - (float)(BOUNCE_CONST2 * Math.Pow(t - (1.5 / BOUNCE_CONST1), 2) + .75);
            if (t < (2.5 / BOUNCE_CONST1))
                return 1 - (float)(BOUNCE_CONST2 * Math.Pow(t - (2.25 / BOUNCE_CONST1), 2) + .9375);
            //small bounce
            return 1 - (float)(BOUNCE_CONST2 * Math.Pow(t - (2.625 / BOUNCE_CONST1), 2) + .984375);
        }
    }
}