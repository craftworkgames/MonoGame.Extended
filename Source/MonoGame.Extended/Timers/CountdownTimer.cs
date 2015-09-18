using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended.Timers
{
    public class CountdownTimer : IGameComponent
    {
        public event EventHandler Tick;
        public event EventHandler Completed;

        public TimerState TimerState;
        public TimeSpan CurrentTick { get; set; } = TimeSpan.Zero;
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;
        public TimeSpan TimeRemaining
        {
            get { return Interval - CurrentTick; }
        }

        public CountdownTimer(TimeSpan interval)
        {
            Interval = interval;
            Reset();
        }

        void IGameComponent.Initialize() { Reset(); }
        public void Reset()
        {
            TimerState = TimerState.Stopped;
            CurrentTick = TimeSpan.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (TimerState == TimerState.Stopped ||
                TimerState == TimerState.Paused ||
                TimerState == TimerState.Completed)
                return;

            CurrentTick += gameTime.ElapsedGameTime;

            if (CurrentTick < Interval)
            {
                if (Tick != null)
                    Tick(this, null);
            }
            else
            {
                TimerState = TimerState.Completed;
                CurrentTick = Interval;

                if (Completed != null)
                    Completed(this, null);
            }
        }
    }
}
