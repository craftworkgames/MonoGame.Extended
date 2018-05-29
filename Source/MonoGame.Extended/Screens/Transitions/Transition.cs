using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens.Transitions
{
    public enum TransitionState { In, Out }

    public abstract class Transition : IDisposable
    {
        private readonly float _halfDuration;
        private float _currentSeconds;
        
        protected Transition(float duration)
        {
            Duration = duration;
            _halfDuration = Duration / 2f;
        }

        public abstract void Dispose();

        public TransitionState State { get; private set; } = TransitionState.Out;
        public float Duration { get; }
        public float Value => MathHelper.Clamp(_currentSeconds / _halfDuration, 0f, 1f);

        public event EventHandler StateChanged;
        public event EventHandler Completed;

        public void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            switch (State)
            {
                case TransitionState.Out:
                    _currentSeconds += elapsedSeconds;

                    if (_currentSeconds >= _halfDuration)
                    {
                        State = TransitionState.In;
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case TransitionState.In:
                    _currentSeconds -= elapsedSeconds;

                    if (_currentSeconds <= 0.0f)
                    {
                        Completed?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract void Draw(GameTime gameTime);
    }
}