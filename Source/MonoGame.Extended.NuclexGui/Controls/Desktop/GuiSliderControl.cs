using System;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Base class for a slider that can be moved using the mouse</summary>
    /// <remarks>
    ///     Implements the common functionality for a slider moving either the direction
    ///     of the X or the Y axis (but not both). Derive any scroll bar-like controls
    ///     from this class to simplify their implementation.
    /// </remarks>
    public abstract class GuiSliderControl : GuiControl
    {
        /// <summary>Whether the mouse cursor is hovering over the thumb</summary>
        private bool _mouseOverThumb;

        /// <summary>X coordinate at which the thumb was picked up</summary>
        private float _pickupX;

        /// <summary>Y coordinate at which the thumb was picked up</summary>
        private float _pickupY;

        /// <summary>Whether the slider's thumb is currently in the depressed state</summary>
        private bool _pressedDown;

        /// <summary>Can be set by renderers to allow the control to locate its thumb</summary>
        public IThumbLocator ThumbLocator;

        /// <summary>Position of the thumb within the slider (0.0 .. 1.0)</summary>
        public float ThumbPosition;

        /// <summary>Fraction of the slider filled by the thumb (0.0 .. 1.0)</summary>
        public float ThumbSize;

        /// <summary>Initializes a new slider control</summary>
        public GuiSliderControl()
        {
            ThumbPosition = 0.0f;
            ThumbSize = 1.0f;
        }

        /// <summary>whether the mouse is currently hovering over the thumb</summary>
        public bool MouseOverThumb => _mouseOverThumb;

        /// <summary>Whether the pressable control is in the depressed state</summary>
        public virtual bool ThumbDepressed => _pressedDown && _mouseOverThumb;

        /// <summary>Triggered when the slider has been moved</summary>
        public event EventHandler Moved;

        /// <summary>Moves the thumb to the specified location</summary>
        /// <returns>Location the thumb will be moved to</returns>
        protected abstract void MoveThumb(float x, float y);

        /// <summary>Obtains the region covered by the slider's thumb</summary>
        /// <returns>The region covered by the slider's thumb</returns>
        protected abstract RectangleF GetThumbRegion();

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        protected override void OnMousePressed(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                var thumbRegion = GetThumbRegion();
                if (thumbRegion.Contains(new Point2(_pickupX, _pickupY)))
                {
                    _pressedDown = true;

                    _pickupX -= thumbRegion.X;
                    _pickupY -= thumbRegion.Y;
                }
            }
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        protected override void OnMouseReleased(MouseButton button)
        {
            if (button == MouseButton.Left)
                _pressedDown = false;
        }

        /// <summary>Called when the mouse position is updated</summary>
        /// <param name="x">X coordinate of the mouse cursor on the control</param>
        /// <param name="y">Y coordinate of the mouse cursor on the control</param>
        protected override void OnMouseMoved(float x, float y)
        {
            if (_pressedDown)
            {
                //RectangleF bounds = GetAbsoluteBounds();
                MoveThumb(x - _pickupX, y - _pickupY);
            }
            else
            {
                _pickupX = x;
                _pickupY = y;
            }

            _mouseOverThumb = GetThumbRegion().Contains(new Point2(x, y));
        }

        /// <summary>
        ///     Called when the mouse has left the control and is no longer hovering over it
        /// </summary>
        protected override void OnMouseLeft()
        {
            _mouseOverThumb = false;
        }

        /// <summary>Fires the slider's Moved event</summary>
        protected virtual void OnMoved()
        {
            if (Moved != null)
                Moved(this, EventArgs.Empty);
        }
    }
}