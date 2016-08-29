using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounterComponent : DrawableGameComponent
    {
        private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();

        public FramesPerSecondCounterComponent(Game game)
            : base(game)
        {
        }

        public float FramesPerSecond => _fpsCounter.FramesPerSecond;

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

