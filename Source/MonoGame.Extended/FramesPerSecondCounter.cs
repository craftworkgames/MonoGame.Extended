using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounter : IUpdate
    {
        private static readonly TimeSpan OneSecondTimeSpan = new TimeSpan(0, 0, 1);
        private TimeSpan _timer = OneSecondTimeSpan;
        private int _framesCounter;
        private readonly Deque<float> _sampleBuffer;

        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }
        public int MaximumSamples { get; }

        public FramesPerSecondCounter(int maximumSamples = 1)
        {
            if (maximumSamples < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumSamples));
            }

            MaximumSamples = maximumSamples;
            _sampleBuffer = new Deque<float>(maximumSamples);
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;
            if (_timer <= OneSecondTimeSpan)
                return;

            CurrentFramesPerSecond = _framesCounter;
            _framesCounter = 0;
            _timer = TimeSpan.Zero;

            if (_sampleBuffer.Count >= MaximumSamples)
            {
                _sampleBuffer.RemoveFromFront();
            }

            _sampleBuffer.AddToBack(CurrentFramesPerSecond);

            AverageFramesPerSecond = _sampleBuffer.Average(i => i);
        }

        public void Draw(GameTime gameTime)
        {
            _framesCounter++;
        }
    }
}
