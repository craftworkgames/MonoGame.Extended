namespace MonoGame.Extended.Tweening;

/// <summary>
/// A tween that animates a value of type <typeparamref name="T"/> using linear interpolation
/// via the arithmetic operations provided by <see cref="LinearOperations{T}"/>.
/// </summary>
/// <typeparam name="T">
/// The value type to animate. Must support the addition, subtraction, and scalar multiplication
/// operators required by <see cref="LinearOperations{T}"/>.
/// </typeparam>
public class LinearTween<T>: Tween<T>
    where T: struct
{
    private T _range;

    internal LinearTween(object target, float duration, float delay, TweenMember<T> member, T endValue) : base(target, duration, delay, member, endValue)
    {
    }

    /// <summary>
    /// Captures the initial member value as the start value and computes the animation range.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        _range = LinearOperations<T>.Subtract(_endValue, _startValue);
    }

    /// <summary>
    /// Interpolates the member value between the start and end values for the given progress.
    /// </summary>
    /// <param name="n">The interpolation progress in the range [0, 1], after easing has been applied.</param>
    protected override void Interpolate(float n)
    {
        var value = LinearOperations<T>.Add(_startValue, LinearOperations<T>.Multiply(_range, n));
        Member.Value = value;
    }
}
