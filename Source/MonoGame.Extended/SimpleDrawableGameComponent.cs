using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class SimpleDrawableGameComponent : SimpleGameComponent, IDrawable
    {
        private bool _isVisible;
        private int _drawOrder;

        public bool Visible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        bool IDrawable.Visible
        {
            get { return _isVisible; }
        }

        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder == value)
                {
                    return;
                }

                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        protected SimpleDrawableGameComponent()
        {
        }

        public abstract void Draw(GameTime gameTime);
    }
}
