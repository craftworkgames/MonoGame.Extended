using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class ContinuousClock : IGameComponent
    {
        public ContinuousClock(double tickSeconds)
        {
            TickSeconds = tickSeconds;
            Reset();
        }

        public event EventHandler Tick;

        public TimerState State { get; protected set; }
        public double TickSeconds { get; private set; }
        public TimeSpan CurrentTime { get; protected set; }
        public TimeSpan NextReport { get; protected set; }

        void IGameComponent.Initialize()
        {
            Reset();
        }

        public void Reset()
        {
            State = TimerState.Stopped;
            CurrentTime = TimeSpan.Zero;
            NextReport = CurrentTime + TimeSpan.FromSeconds(TickSeconds);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (State == TimerState.Stopped || State == TimerState.Paused)
                return;

            CurrentTime += gameTime.ElapsedGameTime;

            if (CurrentTime >= NextReport)
            {
                NextReport = CurrentTime + TimeSpan.FromSeconds(TickSeconds);
                Tick.Raise(this, EventArgs.Empty);
            }
        }
    }
}
