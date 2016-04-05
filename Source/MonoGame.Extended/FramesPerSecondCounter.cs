using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounter : DrawableGameComponent
    {
        public FramesPerSecondCounter(Game game, int maximumSamples = 100)
            :base(game)
        {
            MaximumSamples = maximumSamples;
        }

        private readonly Queue<float> _sampleBuffer = new Queue<float>();

        public long TotalFrames { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; } 
        public int MaximumSamples { get; }

        public void Reset()
        {
            TotalFrames = 0;
            _sampleBuffer.Clear();
        }

        public void UpdateFPS(float deltaTime)
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateFPS((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(gameTime);
        }
    }
}

