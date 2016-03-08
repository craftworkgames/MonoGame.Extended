using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class CountdownTimer : GameTimer
    {
        public event EventHandler Completed;

        public event EventHandler TimeRemainingChanged;

        public TimeSpan TimeRemaining { get; private set; }

        public CountdownTimer(double intervalSeconds)
            : base(intervalSeconds)
        {
        }

        public CountdownTimer(TimeSpan interval)
            : base(interval)
        {
        }

        protected override void OnStopped()
        {
            CurrentTime = TimeSpan.Zero;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            TimeRemaining = Interval - CurrentTime;
            TimeRemainingChanged.Raise(this, EventArgs.Empty);

            if (CurrentTime >= Interval)
            {
                State = TimerState.Completed;
                CurrentTime = Interval;
                TimeRemaining = TimeSpan.Zero;
                Completed.Raise(this, EventArgs.Empty);
            }
        }
    }
}