using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounterComponent : DrawableGameComponent
    {
        private readonly FramesPerSecondCounter _fpsCounter;

        public FramesPerSecondCounterComponent(Game game, int maximumSamples = 1)
            : base(game)
        {
            _fpsCounter = new FramesPerSecondCounter(maximumSamples);
        }

        public float AverageFramesPerSecond => _fpsCounter.AverageFramesPerSecond;

        public float CurrentFramesPerSecond => _fpsCounter.CurrentFramesPerSecond;

        public int MaximumSamples => _fpsCounter.MaximumSamples;

        public override void Update(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _fpsCounter.Draw(gameTime);
        }
    }
}

