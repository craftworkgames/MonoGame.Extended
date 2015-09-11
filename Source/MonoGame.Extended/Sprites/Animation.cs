using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class Animation
    {
        private void Initalize()
        {
            _sprites = new List<Sprite>();
            _timeElapsed = 0;
            _frameIndex = 0;
            _speed = 0;
            _playing = false;

            Loop = true;
        }

        public Animation(Sprite[] sprites)
        {
            Initalize();
            _sprites.AddRange(sprites);
        }

        public Animation(Texture2D[] textures)
        {
            Initalize();
            foreach (var t in textures)
                _sprites.Add(new Sprite(t));
        }

        public Animation(Texture2D texture, Vector2 startpoint, float framewidth, float frameheight, int rows, int cols, float rowborder, float colborder)
        {
            Initalize();
            for (float x = startpoint.X; x < startpoint.X + framewidth * cols; x += framewidth + colborder)
                for (float y = startpoint.Y; y < startpoint.Y + frameheight * rows; y += frameheight + rowborder)
                    _sprites.Add(new Sprite(new TextureRegion2D(texture, (int)x, (int)y, (int)framewidth, (int)frameheight)));
        }

        internal List<Sprite> _sprites;
        private float _timeElapsed;
        public bool Loop { get; set; }

        public Sprite CurrentSprite
        {
            get
            {
                return _sprites[_frameIndex];
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
                    throw new ArgumentOutOfRangeException("value", "AnimationSpeed has to be equal or higher than 0");

                _speed = value;
            }
        }

        private int _frameIndex;
        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "FrameIndex has to be equal or higher than 0");

                if (value >= _sprites.Count)
                    throw new ArgumentOutOfRangeException("value", "FrameIndex cannot be higher than the ammount of frames that the sprite contains.");

                _frameIndex = value;
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

                    if (_frameIndex == _sprites.Count)
                    {
                        if (Loop)
                            _frameIndex = 0;
                        else
                        {
                            _frameIndex--;
                            _playing = false;
                        }
                    }

                    _timeElapsed = 0;
                }
            }
        }

        public void Play()
        {
            _playing = true;
            _timeElapsed = 0;

            if (!Loop && _frameIndex == _sprites.Count - 1)
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

