using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class CountdownTimer
    {
        public CountdownTimer(TimeSpan interval)
        {
            Interval = interval;
            Reset();
        }

        public event EventHandler Tick;
        public event EventHandler Completed;

        public TimerState TimerState { get; private set; }
        public TimeSpan CurrentTick { get; set; }
        public TimeSpan Interval { get; set; }

        public TimeSpan TimeRemaining
        {
            get { return Interval - CurrentTick; }
        }

        public void Reset()
        {
            TimerState = TimerState.Stopped;
            CurrentTick = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (TimerState == TimerState.Stopped || TimerState == TimerState.Paused || TimerState == TimerState.Completed)
                return;

            CurrentTick += gameTime.ElapsedGameTime;

            if (CurrentTick < Interval)
            {
                Tick.Raise(this, EventArgs.Empty);
            }
            else
            {
                TimerState = TimerState.Completed;
                CurrentTick = Interval;
                Completed.Raise(this, EventArgs.Empty);
            }
        }
    }
}
