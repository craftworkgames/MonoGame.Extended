using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            _sprite.TextureRegion = _textureAtlas._regions[_frameIndex];
        }

        private Sprite _sprite;
        private TextureAtlas _textureAtlas;
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
                _sprite.TextureRegion = _textureAtlas._regions[_frameIndex];
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

        private bool _playing;
        public bool Playing
        {
            get { return _playing; }
        }

        public void Update(GameTime gameTIme)
        {
            if (_playing && Speed > 0)
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
                            _playing = false;
                        }
                    }

                    _timeElapsed -= Speed;
                    _sprite.TextureRegion = _textureAtlas._regions[_frameIndex];
                }
            }
        }

        public void Play()
        {
            _playing = true;
            _timeElapsed = 0;

            if (!Loop && _frameIndex == _textureAtlas.RegionCount - 1)
                _frameIndex = 0;
        }

        public void Pause()
        {
            _playing = false;
        }

        public void Stop()
        {
            _playing = false;
            _frameIndex = 0;
        }
    }
}

