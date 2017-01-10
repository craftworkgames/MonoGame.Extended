using System;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimation : Animation
    {
        public const float DefaultFrameDuration = 0.2f;

        public SpriteSheetAnimation(string name, TextureAtlas textureAtlas, float frameDuration = DefaultFrameDuration,
            bool isLooping = true, bool isReversed = false, bool isPingPong = false)
            : this(name, textureAtlas.Regions.ToArray(), frameDuration, isLooping, isReversed, isPingPong)
        {
        }

        public SpriteSheetAnimation(string name, TextureRegion2D[] keyFrames, float frameDuration = DefaultFrameDuration,
            bool isLooping = true, bool isReversed = false, bool isPingPong = false)
            : base(null, false)
        {
            Name = name;
            KeyFrames = keyFrames;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            IsReversed = isReversed;
            IsPingPong = isPingPong;
            CurrentFrameIndex = IsReversed ? KeyFrames.Length - 1 : 0;
        }

        public SpriteSheetAnimation(string name, TextureRegion2D[] keyFrames, SpriteSheetAnimationData data)
            : this(name, keyFrames, data.FrameDuration, data.IsLooping, data.IsReversed, data.IsPingPong)
        {
        }

        public string Name { get; }
        public TextureRegion2D[] KeyFrames { get; }
        public float FrameDuration { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }
        public new bool IsComplete => CurrentTime >= AnimationDuration;

        public float AnimationDuration => IsPingPong
            ? (KeyFrames.Length*2 - 2)*FrameDuration
            : KeyFrames.Length*FrameDuration;

        public TextureRegion2D CurrentFrame => KeyFrames[CurrentFrameIndex];
        public int CurrentFrameIndex { get; private set; }

        public float FramesPerSecond
        {
            get { return 1.0f/FrameDuration; }
            set { FrameDuration = value/1.0f; }
        }

        public Action OnCompleted { get; set; }

        protected override bool OnUpdate(float deltaTime)
        {
            if (IsComplete)
            {
                OnCompleted?.Invoke();

                if (IsLooping)
                    CurrentTime -= AnimationDuration;
            }

            if (KeyFrames.Length == 1)
            {
                CurrentFrameIndex = 0;
                return IsComplete;
            }

            var frameIndex = (int) (CurrentTime/FrameDuration);
            var length = KeyFrames.Length;

            if (IsPingPong)
            {
                frameIndex = frameIndex%(length*2 - 2);

                if (frameIndex >= length)
                    frameIndex = length - 2 - (frameIndex - length);
            }

            if (IsLooping)
            {
                if (IsReversed)
                {
                    frameIndex = frameIndex%length;
                    frameIndex = length - frameIndex - 1;
                }
                else
                    frameIndex = frameIndex%length;
            }
            else
                frameIndex = IsReversed ? Math.Max(length - frameIndex - 1, 0) : Math.Min(length - 1, frameIndex);

            CurrentFrameIndex = frameIndex;
            return IsComplete;
        }
    }
}