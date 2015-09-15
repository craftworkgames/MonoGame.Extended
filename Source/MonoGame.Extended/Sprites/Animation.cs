using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class Animation
    {
        public Animation(Sprite[] sprites)
        {
            Loop = true;

            _sprites = new List<Sprite>();
            _sprites.AddRange(sprites);
        }

        public Animation(Texture2D[] textures)
        {
            Loop = true;

            _sprites = new List<Sprite>();
            foreach (var t in textures)
                _sprites.Add(new Sprite(t));
        }

        public Animation(Texture2D texture, Vector2 startpoint, float framewidth, float frameheight, int rows, int cols, float rowborder, float colborder, bool lefttop)
        {
            Loop = true;

            _sprites = new List<Sprite>();

            if (lefttop)
            {
                for (float x = startpoint.X; x < startpoint.X + framewidth * cols; x += framewidth + colborder)
                    for (float y = startpoint.Y; y < startpoint.Y + frameheight * rows; y += frameheight + rowborder)
                        _sprites.Add(new Sprite(new TextureRegion2D(texture, (int)x, (int)y, (int)framewidth, (int)frameheight)));
            }
            else
            {
                for (float y = startpoint.Y; y < startpoint.Y + frameheight * rows; y += frameheight + rowborder)
                    for (float x = startpoint.X; x < startpoint.X + framewidth * cols; x += framewidth + colborder)
                        _sprites.Add(new Sprite(new TextureRegion2D(texture, (int)x, (int)y, (int)framewidth, (int)frameheight)));
            }
        }

        private float _timeElapsed;
        public bool Loop { get; set; }

        public Vector2 Position { get; set; }

        public Sprite CurrentSprite
        {
            get
            {
                return _sprites[_frameIndex];
            }
        }

        public int FrameCount
        {
            get
            {
                return _sprites.Count;
            }
        }

        private List<Sprite> _sprites;
        public List<Sprite> Sprites
        {
            get
            {
                return _sprites;
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

        private int _frameIndex;
        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "FrameIndex has to be equal or higher than 0.");

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

                    _timeElapsed -= Speed;
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

        public void AddSprite(Sprite sprite)
        {
            AddSprite(sprite, _sprites.Count);
        }

        public void AddSprite(Sprite sprite, int position)
        {
            _sprites.Insert(position, sprite);
        }

        public void RemoveSprite(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "index has to be equal or higher than 0.");

            if (index >= _sprites.Count)
                throw new ArgumentOutOfRangeException("index", "index cannot be higher than the ammount of frames that the sprite contains.");

            RemoveSprite(_sprites[index]);
        }

        public void RemoveSprite(Sprite sprite)
        {
            if (Sprites.Count == 1)
                throw new Exception("Animation has to contain at least one sprite.");

            _sprites.Remove(sprite);
        }
    }
}

