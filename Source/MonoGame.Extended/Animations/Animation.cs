using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public abstract class Animation : IUpdate, IDisposable
    {
        private readonly bool _disposeOnComplete;

        private readonly Action _onCompleteAction;

        private bool _isComplete;

        protected Animation(Action onCompleteAction, bool disposeOnComplete)
        {
            _onCompleteAction = onCompleteAction;
            _disposeOnComplete = disposeOnComplete;
            IsPaused = false;
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            protected set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;

                    if (_isComplete)
                    {
                        _onCompleteAction?.Invoke();

                        if (_disposeOnComplete)
                            Dispose();
                    }
                }
            }
        }

        public bool IsDisposed { get; private set; }
        public bool IsPlaying => !IsPaused && !IsComplete;
        public bool IsPaused { get; private set; }
        public float CurrentTime { get; protected set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }

        public void Play()
        {
            IsPaused = false;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Stop()
        {
            Pause();
            Rewind();
        }

        public void Rewind()
        {
            CurrentTime = 0;
        }

        protected abstract bool OnUpdate(float deltaTime);

        public void Update(float deltaTime)
        {
            if (!IsPlaying)
                return;

            CurrentTime += deltaTime;
            IsComplete = OnUpdate(deltaTime);
        }
    }
}