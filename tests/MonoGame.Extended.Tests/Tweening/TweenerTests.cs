using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tweening.Tests;

public class TweenerTests
{
    [Fact]
    public void TweenerTweenToSuccessTest()
    {
        var tweener = new Tweener();
        var obj = new ColorHandler();
        tweener.TweenTo(obj, x => x.Color, Color.Red, 2f);
    }

    [Fact]
    public void ActiveTweens_EmptyWhenNoTweensStarted()
    {
        var tweener = new Tweener();
        Assert.Equal(0, tweener.ActiveTweens.Length);
    }

    [Fact]
    public void ActiveTweens_ReflectsNumberOfRunningTweens()
    {
        var tweener = new Tweener();
        var obj = new FloatHandler();

        tweener.TweenTo(obj, x => x.Value, 10f, 2f);
        Assert.Equal(1, tweener.ActiveTweens.Length);

        tweener.TweenTo(obj, x => x.Other, 5f, 2f);
        Assert.Equal(2, tweener.ActiveTweens.Length);
    }

    [Fact]
    public void ActiveTweens_DecreasesAfterTweenCompletes()
    {
        var tweener = new Tweener();
        var obj = new FloatHandler();

        tweener.TweenTo(obj, x => x.Value, 10f, 1f);
        Assert.Equal(1, tweener.ActiveTweens.Length);

        // Advance past the duration so the tween is removed on Update.
        tweener.Update(2f);
        Assert.Equal(0, tweener.ActiveTweens.Length);
    }

    [Fact]
    public void OnUpdate_FiresAfterEachUpdateWithInterpolatedValue()
    {
        var tweener = new Tweener();
        var obj = new FloatHandler();
        var callCount = 0;
        var recordedValues = new List<float>();

        tweener.TweenTo(obj, x => x.Value, 10f, 1f)
            .OnUpdate(_ => { callCount++; recordedValues.Add(obj.Value); });

        tweener.Update(0.5f);
        Assert.Equal(1, callCount);
        Assert.Equal(5f, recordedValues[0]);

        tweener.Update(0.5f);
        Assert.Equal(2, callCount);
        Assert.Equal(10f, recordedValues[1]);
    }

    [Fact]
    public void ActiveTweens_EmptyAfterCancelAll()
    {
        var tweener = new Tweener();
        var obj = new FloatHandler();

        tweener.TweenTo(obj, x => x.Value, 10f, 2f);
        tweener.CancelAll();
        tweener.Update(0f);

        Assert.Equal(0, tweener.ActiveTweens.Length);
    }
}

file class FloatHandler
{
    public float Value { get; set; }
    public float Other { get; set; }
}
