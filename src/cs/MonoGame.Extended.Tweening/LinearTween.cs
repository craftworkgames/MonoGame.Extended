namespace MonoGame.Extended.Tweening;

public class LinearTween<T>: Tween<T>
    where T: struct
{
    private T _range;

    internal LinearTween(object target, float duration, float delay, TweenMember<T> member, T endValue) : base(target, duration, delay, member, endValue)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        _range = LinearOperations<T>.Subtract(_endValue, _startValue);
    }

    protected override void Interpolate(float n)
    {
        var value = LinearOperations<T>.Add(_startValue, LinearOperations<T>.Multiply(_range, n));
        Member.Value = value;
    }
}
