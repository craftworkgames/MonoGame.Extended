using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Timers
{
    public class ContinuousClock : GameTimer
    {
        public ContinuousClock(double intervalSeconds)
            : base(intervalSeconds)
        {
        }

        public ContinuousClock(TimeSpan interval)
            : base(interval)
        {
        }

        public event EventHandler Tick;

        public TimeSpan NextTickTime { get; protected set; }

        protected override void OnStopped()
        {
            NextTickTime = CurrentTime + Interval;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (CurrentTime >= NextTickTime)
            {
                NextTickTime = CurrentTime + Interval;
                Tick.Raise(this, EventArgs.Empty);
            }
        }
    }
}
