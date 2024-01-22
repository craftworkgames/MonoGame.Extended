using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening;

public class ColorTween: Tween<Color>
{
    internal ColorTween(object target, float duration, float delay, TweenMember<Color> member, Color endValue) : base(target, duration, delay, member, endValue)
    {
    }

    protected override void Interpolate(float n)
    {
        Member.Value = Color.Lerp(_startValue, _endValue, n);
    }
}
