using System;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    //[EntityComponent]
    //[EntityComponentPool(InitialSize = 100, IsFullPolicy = ObjectPoolIsFullPolicy.IncreaseSize)]
    //public class AnimationComponent : EntityComponent
    //{
    //    public string CurrentAnimationName { get; private set; }

    //    public override void Reset()
    //    {
    //        CurrentAnimationName = null;
    //    }

    //    public SpriteSheetAnimation Play(string name, Action onCompleted = null)
    //    {
    //        if (CurrentAnimationName != null && !CurrentAnimationName.IsComplete && CurrentAnimationName.Name == name)
    //            return CurrentAnimationName;

    //        CurrentAnimationName = AnimationFactory.Create(name);
    //        CurrentAnimationName.OnCompleted = onCompleted;

    //        return CurrentAnimationName;
    //    }
    //}
}
