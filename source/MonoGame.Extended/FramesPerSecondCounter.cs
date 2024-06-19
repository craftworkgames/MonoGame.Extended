using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public class FramesPerSecondCounter : IUpdateable
    {
        private bool _enabled;
        private int _updateOrder;

        private static readonly TimeSpan _oneSecondTimeSpan = new TimeSpan(0, 0, 1);
        private int _framesCounter;
        private TimeSpan _timer = _oneSecondTimeSpan;

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
                _updateOrder= value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs> EnabledChanged;

        /// <inheritdoc />
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public FramesPerSecondCounter()
        {
        }

        public int FramesPerSecond { get; private set; }


        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;
            if (_timer <= _oneSecondTimeSpan)
                return;

            FramesPerSecond = _framesCounter;
            _framesCounter = 0;
            _timer -= _oneSecondTimeSpan;
        }

        public void Draw(GameTime gameTime)
        {
            _framesCounter++;
        }
    }
}
