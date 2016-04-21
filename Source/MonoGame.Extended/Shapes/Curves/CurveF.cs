using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Shapes.Curves
{
    public abstract class CurveF : IPathF
    {
        public virtual Vector2 StartPoint { get; set; }
        public virtual Vector2 EndPoint { get; set; }
        public abstract float Length { get; protected set; }
        
        /// <summary>
        /// Returns the point at t on the curve, 1 being startpoint and 0 being endpoint
        /// </summary>
        /// <param name="t">a value between 0 and 1</param>
        /// <returns></returns>
        public abstract Vector2 GetPointOnCurve(float t);
        
    }
}