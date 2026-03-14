using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening;

/// <summary>
/// A tween that animates a <see cref="Color"/> value using <see cref="Color.Lerp"/> for interpolation.
/// </summary>
public class ColorTween: Tween<Color>
{
    internal ColorTween(object target, float duration, float delay, TweenMember<Color> member, Color endValue) : base(target, duration, delay, member, endValue)
    {
    }

    /// <summary>
    /// Interpolates the member's color value between the start and end colors for the given progress.
    /// </summary>
    /// <param name="n">The interpolation progress in the range [0, 1], after easing has been applied.</param>
    protected override void Interpolate(float n)
    {
        Member.Value = Color.Lerp(_startValue, _endValue, n);
    }
}
