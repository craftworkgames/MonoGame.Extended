﻿using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    [Obsolete("Please use the SpriteSheetAnimator instead")]
    public class SpriteAnimator : IUpdate
    {
        private readonly Sprite _sprite;
        private readonly TextureAtlas _textureAtlas;

        private int _frameIndex;

        private float _framesPerSecond;
        private float _nextFrameDelay;

        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "FrameIndex has to be equal or higher than 0.");
                }

                if (value >= _textureAtlas.RegionCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "FrameIndex cannot be higher than the ammount of frames that the sprite contains.");
                }

                _frameIndex = value;
                _sprite.TextureRegion = _textureAtlas[_frameIndex];
            }
        }

        public float FramesPerSecond
        {
            get { return _framesPerSecond; }
            set
            {
                if (_framesPerSecond < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "FramesPerSecond has to be equal or higher than 0.");
                }

                _framesPerSecond = value;
            }
        }

        public bool IsLooping { get; set; }

        public bool IsPlaying { get; set; }

        public SpriteAnimator(Sprite sprite, TextureAtlas textureAtlas, int framesPerSecond = 60)
        {
            IsLooping = true;
            IsPlaying = true;
            FramesPerSecond = framesPerSecond;

            _sprite = sprite;
            _textureAtlas = textureAtlas;
            _frameIndex = 0;
            _nextFrameDelay = 0;
            _sprite.TextureRegion = _textureAtlas[_frameIndex];
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying && FramesPerSecond > 0)
            {
                var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_nextFrameDelay <= 0)
                {
                    _nextFrameDelay = 1.0f / FramesPerSecond;
                    _frameIndex++;

                    if (_frameIndex >= _textureAtlas.RegionCount)
                    {
                        if (IsLooping)
                        {
                            _frameIndex = 0;
                        }
                        else
                        {
                            IsPlaying = false;
                        }
                    }

                    _sprite.TextureRegion = _textureAtlas[_frameIndex];
                }

                _nextFrameDelay -= deltaSeconds;
            }
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Stop()
        {
            IsPlaying = false;
            _frameIndex = 0;
        }
    }
}