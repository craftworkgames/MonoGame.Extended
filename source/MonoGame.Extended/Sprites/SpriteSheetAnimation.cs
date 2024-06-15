﻿using System;
using System.Linq;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Sprites
{
    public class SpriteSheetAnimation : Animation
    {
        public const float DefaultFrameDuration = 0.2f;

        public SpriteSheetAnimation(string name, Texture2DAtlas textureAtlas, float frameDuration = DefaultFrameDuration,
            bool isLooping = true, bool isReversed = false, bool isPingPong = false)
            : this(name, textureAtlas.ToArray(), frameDuration, isLooping, isReversed, isPingPong)
        {
        }

        public SpriteSheetAnimation(string name, Texture2DRegion[] keyFrames, float frameDuration = DefaultFrameDuration,
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

        public SpriteSheetAnimation(string name, Texture2DRegion[] keyFrames, SpriteSheetAnimationData data)
            : this(name, keyFrames, data.FrameDuration, data.IsLooping, data.IsReversed, data.IsPingPong)
        {
        }

        public string Name { get; }
        public Texture2DRegion[] KeyFrames { get; }
        public float FrameDuration { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }
        public new bool IsComplete => CurrentTime >= AnimationDuration;

        public float AnimationDuration => IsPingPong
            ? (KeyFrames.Length*2 - (IsLooping ? 2 : 1))*FrameDuration
            : KeyFrames.Length*FrameDuration;

        public Texture2DRegion CurrentFrame => KeyFrames[CurrentFrameIndex];
        public int CurrentFrameIndex { get; private set; }

        public float FramesPerSecond
        {
            get => 1.0f/FrameDuration;
            set => FrameDuration = value/1.0f;
        }

        public Action OnCompleted { get; set; }

        protected override bool OnUpdate(float deltaTime)
        {
            if (IsComplete)
            {
                if (IsLooping)
                    CurrentTime %= AnimationDuration;
                else
                    OnCompleted?.Invoke();
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
                if (IsComplete)
                    frameIndex = 0;
                else
                {
                    frameIndex = frameIndex % (length * 2 - 2);

                    if (frameIndex >= length)
                        frameIndex = length - 2 - (frameIndex - length);
                }
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
