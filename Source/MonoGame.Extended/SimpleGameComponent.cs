using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class SimpleGameComponent : IGameComponent, IUpdateable
    {
        private bool _isEnabled = true;
        private int _updateOrder;

        protected SimpleGameComponent()
        {
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;

                _isEnabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual void Initialize()
        {
        }

        bool IUpdateable.Enabled => _isEnabled;

        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder == value)
                    return;

                _updateOrder = value;
                UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public abstract void Update(GameTime gameTime);
    }
}