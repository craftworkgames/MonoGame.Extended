using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class SimpleDrawableGameComponent : SimpleGameComponent, IDrawable
    {
        private int _drawOrder;
        private bool _isVisible;

        protected SimpleDrawableGameComponent()
        {
        }

        public bool Visible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        bool IDrawable.Visible => _isVisible;

        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder == value)
                    return;

                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public abstract void Draw(GameTime gameTime);

        protected virtual void LoadContent()
        {
        }

        protected virtual void UnloadContent()
        {
        }
    }
}