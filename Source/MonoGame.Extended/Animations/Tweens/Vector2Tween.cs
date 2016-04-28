using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class Vector2Tween : Tween<Vector2>
    {
        public Vector2Tween(Vector2 initialValue, Action<Vector2> setValue, Vector2 targetValue, float duration, EasingFunction easingFunction) 
            : base(initialValue, setValue, targetValue, duration, easingFunction)
        {
        }

        protected override Vector2 CalculateNewValue(Vector2 initialValue, float multiplier)
        {
            return InitialValue + (TargetValue - InitialValue) * multiplier;
        }
    }
}