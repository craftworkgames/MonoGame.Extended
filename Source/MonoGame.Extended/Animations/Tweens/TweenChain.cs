using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    //public class TweenChain<T> : TweenAnimation<T>
    //{
    //    public TweenChain(T target, Action onComplete = null)
    //        : base(target, onComplete)
    //    {
    //        _currentTweenIndex = 0;
    //    }

    //    private int _currentTweenIndex;

    //    public override void Update(GameTime gameTime)
    //    {
    //        if(IsComplete)
    //            return;

    //        var currentTween = Tweens[_currentTweenIndex];

    //        currentTween.Update(gameTime);

    //        if (currentTween.IsComplete)
    //            _currentTweenIndex++;

    //        if (_currentTweenIndex >= Tweens.Count)
    //            IsComplete = true;
    //    }
    //}
}