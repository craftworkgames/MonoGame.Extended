using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class ContinuousClock
    {
        public ContinuousClock(double tickSeconds)
        {
            TickSeconds = tickSeconds;
            Restart();
        }

        public event EventHandler Tick;

        public double TickSeconds { get; set; }
        public TimerState State { get; protected set; }
        public TimeSpan CurrentTime { get; protected set; }
        public TimeSpan NextTickTime { get; protected set; }

        public void Start()
        {
            State = TimerState.Started;
        }

        public void Stop()
        {
            State = TimerState.Stopped;
            CurrentTime = TimeSpan.Zero;
            NextTickTime = CurrentTime + TimeSpan.FromSeconds(TickSeconds);
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            State = TimerState.Paused;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (State == TimerState.Stopped || State == TimerState.Paused)
                return;

            CurrentTime += gameTime.ElapsedGameTime;

            if (CurrentTime >= NextTickTime)
            {
                NextTickTime = CurrentTime + TimeSpan.FromSeconds(TickSeconds);
                Tick.Raise(this, EventArgs.Empty);
            }
        }
    }
}
