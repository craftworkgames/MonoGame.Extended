using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounterComponent : DrawableGameComponent
    {
        FramesPerSecondCounter fpsCounter;

        public FramesPerSecondCounterComponent(Game game, int maximumSamples = 100)
            : base(game)
        {
            fpsCounter = new FramesPerSecondCounter(maximumSamples);
        }

        public long TotalFrames
        {
            get { return fpsCounter.TotalFrames; }
        }

        public float AverageFramesPerSecond
        {
            get { return fpsCounter.AverageFramesPerSecond; }
        }

        public float CurrentFramesPerSecond
        {
            get { return fpsCounter.CurrentFramesPerSecond; }
        }

        public int MaximumSamples
        {
            get { return fpsCounter.MaximumSamples; }
        }

        public void Reset()
        {
            fpsCounter.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(gameTime);
        }
    }
}

