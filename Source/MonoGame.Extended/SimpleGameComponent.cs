using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class SimpleGameComponent : IGameComponent, IUpdateable, IDisposable, IComparable<GameComponent>, IComparable<SimpleGameComponent>
    {
        private bool _isInitialized;

        protected SimpleGameComponent()
        {
        }

        public virtual void Dispose()
        {
            if (_isInitialized)
            {
                UnloadContent();
                _isInitialized = false;
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
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
            if (!_isInitialized)
            {
                LoadContent();
                _isInitialized = true;
            }
        }

        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }

        bool IUpdateable.Enabled => _isEnabled;

        private int _updateOrder;
        public int UpdateOrder
        {
            get => _updateOrder;
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
        
        public int CompareTo(GameComponent other)
        {
            return other.UpdateOrder - UpdateOrder;
        }

        public int CompareTo(SimpleGameComponent other)
        {
            return other.UpdateOrder - UpdateOrder;
        }
    }
}