using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class SpriteAnimator
    {
        public SpriteAnimator(Sprite sprite, TextureAtlas textureAtlas)
        {
            Loop = true;
            Speed = 100;

            _sprite = sprite;
            _textureAtlas = textureAtlas;

            _sprite.TextureRegion = _textureAtlas[_frameIndex];
        }

        private readonly Sprite _sprite;
        private readonly TextureAtlas _textureAtlas;
        private float _timeElapsed;

        public bool Loop { get; set; }

        private int _frameIndex;
        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "FrameIndex has to be equal or higher than 0.");

                if (value >= _textureAtlas.RegionCount)
                    throw new ArgumentOutOfRangeException("value", "FrameIndex cannot be higher than the ammount of frames that the sprite contains.");

                _frameIndex = value;
                _sprite.TextureRegion = _textureAtlas[_frameIndex];
            }
        }

        private float _speed;
        public float Speed
        { 
            get
            {
                return _speed;
            }
            set
            {
                if (_speed < 0)
                    throw new ArgumentOutOfRangeException("value", "Speed has to be equal or higher than 0.");

                _speed = value;
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public void Update(GameTime gameTIme)
        {
            if (_isPlaying && Speed > 0)
            {
                _timeElapsed += (float)gameTIme.ElapsedGameTime.TotalMilliseconds;

                if (_timeElapsed > Speed)
                {
                    _frameIndex++;

                    if (_frameIndex == _textureAtlas.RegionCount)
                    {
                        if (Loop)
                            _frameIndex = 0;
                        else
                        {
                            _frameIndex--;
                            _isPlaying = false;
                        }
                    }

                    _timeElapsed -= Speed;
                    _sprite.TextureRegion = _textureAtlas[_frameIndex];
                }
            }
        }

        public void Play()
        {
            _isPlaying = true;
            _timeElapsed = 0;

            if (!Loop && _frameIndex == _textureAtlas.RegionCount - 1)
                _frameIndex = 0;
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Stop()
        {
            _isPlaying = false;
            _frameIndex = 0;
        }
    }
}

