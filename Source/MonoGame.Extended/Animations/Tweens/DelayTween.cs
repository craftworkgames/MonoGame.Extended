using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class DelayTween : IAnimation
    {
        public DelayTween(float duration)
        {
            Duration = duration;
        }

        public float Duration { get; set; }
        public bool IsComplete { get; private set; }
        public bool IsDisposed { get; private set; }

        private float _currentDuration;

        public void Update(GameTime gameTime)
        {
            if(IsComplete)
                return;

            var deltaTime = gameTime.GetElapsedSeconds();

            _currentDuration += deltaTime;

            if (_currentDuration >= Duration)
                IsComplete = true;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}