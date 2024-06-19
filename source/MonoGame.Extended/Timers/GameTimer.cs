using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public abstract class GameTimer : IUpdateable
    {
        private bool _enabled;
        private int _updateOrder;
        /// <inheritdoc />
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                {
                    return;
                }
                _enabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder == value)
                {
                    return;
                }
                _updateOrder = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs> EnabledChanged;

        /// <inheritdoc />
        public event EventHandler<EventArgs> UpdateOrderChanged;

        protected GameTimer(double intervalSeconds)
            : this(TimeSpan.FromSeconds(intervalSeconds))
        {
        }

        protected GameTimer(TimeSpan interval)
        {
            Interval = interval;
            Restart();
        }

        public TimeSpan Interval { get; set; }
        public TimeSpan CurrentTime { get; protected set; }
        public TimerState State { get; protected set; }


        public void Update(GameTime gameTime)
        {
            if (State != TimerState.Started)
                return;

            CurrentTime += gameTime.ElapsedGameTime;
            OnUpdate(gameTime);
        }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler Paused;


        public void Start()
        {
            State = TimerState.Started;
            Started?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            State = TimerState.Stopped;
            CurrentTime = TimeSpan.Zero;
            OnStopped();
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            State = TimerState.Paused;
            Paused?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void OnStopped();
        protected abstract void OnUpdate(GameTime gameTime);
    }
}
