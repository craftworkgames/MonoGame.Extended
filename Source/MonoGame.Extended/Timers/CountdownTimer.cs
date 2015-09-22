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

        public event EventHandler TimeRemainingChanged;
        public event EventHandler Completed;

        public TimeSpan TimeRemaining { get; private set; }

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
