using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class CountdownTimer : GameTimer
    {
        public CountdownTimer(double intervalSeconds)
            : base(intervalSeconds)
        {
        }

        public CountdownTimer(TimeSpan interval)
            : base(interval)
        {
        }

        public TimeSpan TimeRemaining { get; private set; }

        public event EventHandler TimeRemainingChanged;
        public event EventHandler Completed;

        protected override void OnStopped()
        {
            CurrentTime = TimeSpan.Zero;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            TimeRemaining = Interval - CurrentTime;
            TimeRemainingChanged?.Invoke(this, EventArgs.Empty);

            if (CurrentTime >= Interval)
            {
                State = TimerState.Completed;
                CurrentTime = Interval;
                TimeRemaining = TimeSpan.Zero;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}