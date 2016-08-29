using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounter : IUpdate
    {
        private static readonly TimeSpan OneSecondTimeSpan = new TimeSpan(0, 0, 1);
        private TimeSpan _timer = OneSecondTimeSpan;
        private int _framesCounter;

        public int FramesPerSecond { get; private set; }

        public FramesPerSecondCounter()
        {
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;
            if (_timer <= OneSecondTimeSpan)
                return;

            FramesPerSecond = _framesCounter;
            _framesCounter = 0;
            _timer -= OneSecondTimeSpan;
        }

        public void Draw(GameTime gameTime)
        {
            _framesCounter++;
        }
    }
}
