using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class SpriteComponent
    {
        public bool IsVisible { get; set; } = true;

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        public float Depth { get; set; } = 0f;

        public TextureRegion2D TextureRegion { get; set; } = null;
        public Vector2 Origin { get; set; } = Vector2.Zero;

        public float Alpha { get; set; } = 1f;
        public Color Color { get; set; } = Color.White;

        public SpriteEffects Effect { get; set; } = SpriteEffects.None;

        #region Animation

        public bool AnimationChanged { get; private set; } = false;

        public SpriteSheetAnimationFactory AnimationFactory { get; set; } = null;
        public SpriteSheetAnimation CurrentAnimation { get; private set; } = null;

        public SpriteSheetAnimation Play(string name, Action onCompleted = null)
        {
            AnimationChanged = CurrentAnimation == null
                || CurrentAnimation.IsComplete
                || CurrentAnimation.Name != name;

            if (AnimationChanged)
            {
                CurrentAnimation.OnCompleted = null;
                CurrentAnimation = null;

                if (AnimationFactory != null)
                {
                    CurrentAnimation = AnimationFactory.Create(name);
                    CurrentAnimation.OnCompleted = onCompleted;
                }
            }

            return CurrentAnimation;
        }

        #endregion
    }
}
