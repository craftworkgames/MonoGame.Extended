﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounter : IUpdate
    {
        private readonly Queue<float> _sampleBuffer = new Queue<float>();
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }
        public int MaximumSamples { get; }

        public long TotalFrames { get; private set; }

        public FramesPerSecondCounter(int maximumSamples = 100)
        {
            MaximumSamples = maximumSamples;
        }

        public void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Reset()
        {
            TotalFrames = 0;
            _sampleBuffer.Clear();
        }

        public void Update(float deltaTime)
        {
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

            TotalFrames++;
        }
    }
}