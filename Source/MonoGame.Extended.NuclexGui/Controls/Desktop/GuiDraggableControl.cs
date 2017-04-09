using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    // Always move in absolute (offset) coordinates?
    // Or always move in fractional coordinates?
    //
    // Preferring b), because I restores the user's display to the exact
    // state it was if the resolution is changed, including that fact that
    // lower resolutions would cause the windows to go off-screen.
    //
    // However, b) would mean a call to GetAbsolutePosition() each frame.
    // Which isn't so bad, but... avoidable with a)

    // Properties:
    //   Boundaries (for constraining a control to a region)
    //   Moveable (turn moveability on or off)

    /// <summary>Control the user can drag around with the mouse</summary>
    public abstract class GuiDraggableControl : GuiControl
    {
        /// <summary>Whether the control is currently being dragged</summary>
        private bool _beingDragged;

        /// <summary>Whether the control can be dragged</summary>
        private bool _enableDragging;

        /// <summary>X coordinate at which the control was picked up</summary>
        private float _pickupX;

        /// <summary>Y coordinate at which the control was picked up</summary>
        private float _pickupY;

        /// <summary>Initializes a new draggable control</summary>
        public GuiDraggableControl()
        {
            EnableDragging = true;
        }

        /// <summary>Initializes a new draggable control</summary>
        /// <param name="canGetFocus">Whether the control can obtain the input focus</param>
        public GuiDraggableControl(bool canGetFocus) : base(canGetFocus)
        {
            EnableDragging = true;
        }

        /// <summary>Whether the control can be dragged with the mouse</summary>
        protected bool EnableDragging
        {
            get { return _enableDragging; }
            set
            {
                _enableDragging = value;
                _beingDragged &= value;
            }
        }

        /// <summary>Called when the mouse position is updated</summary>
        /// <param name="x">X coordinate of the mouse cursor on the GUI</param>
        /// <param name="y">Y coordinate of the mouse cursor on the GUI</param>
        protected override void OnMouseMoved(float x, float y)
        {
            if (_beingDragged)
            {
                // Adjust the control's position within the container
                var dx = x - _pickupX;
                var dy = y - _pickupY;
                Bounds.AbsoluteOffset(dx, dy);
            }
            else
            {
                // Remember the current mouse position so we know where the user picked
                // up the control when a drag operation begins
                _pickupX = x;
                _pickupY = y;
            }
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        protected override void OnMousePressed(MouseButton button)
        {
            if (button == MouseButton.Left)
                _beingDragged = _enableDragging;
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        protected override void OnMouseReleased(MouseButton button)
        {
            if (button == MouseButton.Left)
                _beingDragged = false;
        }
    }
}