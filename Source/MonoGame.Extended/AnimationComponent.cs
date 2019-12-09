﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Animations
{
    public class AnimationComponent : GameComponent
    {
        public AnimationComponent(Game game)
            : base(game)
        {
            Animations = new List<Animation>();
        }

        public List<Animation> Animations { get; }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (var i = Animations.Count - 1; i >= 0; i--)
            {
                var animation = Animations[i];
                animation.Update(gameTime);
            }

            Animations.RemoveAll(a => a.IsDisposed);
        }
    }
}