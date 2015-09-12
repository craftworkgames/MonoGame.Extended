using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class FPSCounter
    {
        #region Private Fields

        private static int _maximumSamples = 100;
        private static readonly Queue<float> _sampleBuffer = new Queue<float>();

        #endregion

        #region Public Fields

        public static float AverageFramesPerSecond { get; private set; }
        public static float CurrentFramesPerSecond { get; private set; } 

        public static int MaximumSamples
        {
            get { return _maximumSamples; }
            set { _maximumSamples = value; }
        }

        #endregion

        #region Public Methods

        public static void ResetCounter()
        {
            _sampleBuffer.Clear();
        }

        public static void Update(GameTime gameTime)
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

        #endregion
    }
}

