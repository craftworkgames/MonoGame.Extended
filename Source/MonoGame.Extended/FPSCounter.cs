using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FPSCounter
    {
        private readonly Queue<float> _sampleBuffer = new Queue<float>();

        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; } 

        private int _maximumSamples = 100;
        public int MaximumSamples
        {
            get { return _maximumSamples; }
            set { _maximumSamples = value; }
        }

        public void ResetCounter()
        {
            _sampleBuffer.Clear();
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MaximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            } 
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }
        }
    }
}

