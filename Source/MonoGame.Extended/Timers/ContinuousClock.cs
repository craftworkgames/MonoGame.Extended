using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended.Timers
{
    public class ContinuousClock : IGameComponent
    {
        public event EventHandler ReportTimePassed;

        public TimerState TimerState;
        public double ReportEveryXSeconds;

        public TimeSpan CurrentTime { get; protected set; } = TimeSpan.Zero;
        public TimeSpan NextReport { get; protected set; } = TimeSpan.Zero;

        public ContinuousClock(double reportEveryXSeconds)
        {
            ReportEveryXSeconds = reportEveryXSeconds;
            Reset();
        }

        void IGameComponent.Initialize() { Reset(); }
        public void Reset()
        {
            TimerState = TimerState.Stopped;
            CurrentTime = TimeSpan.Zero;
            NextReport = CurrentTime + TimeSpan.FromSeconds(ReportEveryXSeconds);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (TimerState == TimerState.Stopped ||
                TimerState == TimerState.Paused)
                return;

            CurrentTime += gameTime.ElapsedGameTime;

            if (CurrentTime >= NextReport)
            {
                NextReport = CurrentTime + TimeSpan.FromSeconds(ReportEveryXSeconds);

                if (ReportTimePassed != null)
                    ReportTimePassed(this, null);
            }
        }
    }
}
