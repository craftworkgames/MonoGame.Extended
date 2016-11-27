using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>
    /// Determines where the text or image describing control will be located.
    /// Used primarily for radio button and checkbox.
    /// </summary>
    public enum GuiPressableDescriptionPosition
    {
        North,
        South,
        West,
        East
    }

    /// <summary>User interface element the user can push down</summary>
    public abstract class GuiPressableControl : GuiControl, IFocusable
    {
        /// <summary>Whether the mouse is hovering over the command</summary>
        private bool _mouseHovering;

        /// <summary>Whether the command is pressed down using the game pad shortcut</summary>
        private bool _pressedDownByGamepadShortcut;

        /// <summary>Whether the command is pressed down using the space key</summary>
        private bool _pressedDownByKeyboard;

        /// <summary>Whether the command is pressed down using the keyboard shortcut</summary>
        private bool _pressedDownByKeyboardShortcut;

        /// <summary>Whether the command is pressed down using the mouse</summary>
        private bool _pressedDownByMouse;

        /// <summary>Whether the user can interact with the choice</summary>
        public bool Enabled;

        /// <summary>Button that can be pressed to activate this command</summary>
        public Buttons? ShortcutButton;

        /// <summary>Initializes a new command control</summary>
        public GuiPressableControl()
        {
            Enabled = true;
        }

        /// <summary>Whether the mouse pointer is hovering over the control</summary>
        public bool MouseHovering => _mouseHovering;

        /// <summary>Whether the pressable control is in the depressed state</summary>
        public virtual bool Depressed
        {
            get
            {
                var mousePressed = _mouseHovering && _pressedDownByMouse;
                return
                    mousePressed ||
                    _pressedDownByKeyboard ||
                    _pressedDownByKeyboardShortcut ||
                    _pressedDownByGamepadShortcut;
            }
        }

        /// <summary>Whether the control currently has the input focus</summary>
        public bool HasFocus => (Screen != null) &&
                                ReferenceEquals(Screen.FocusedControl, this);

        /// <summary>Whether the control can currently obtain the input focus</summary>
        bool IFocusable.CanGetFocus => Enabled;

        /// <summary>
        ///     Called when the mouse has entered the control and is now hovering over it
        /// </summary>
        protected override void OnMouseEntered()
        {
            _mouseHovering = true;
        }

        /// <summary>
        ///     Called when the mouse has left the control and is no longer hovering over it
        /// </summary>
        protected override void OnMouseLeft()
        {
            // Intentionally not calling OnActivated() here because the user has moved
            // the mouse away from the command while holding the mouse button down -
            // a common trick under windows to last-second-abort the clicking of a button
            _mouseHovering = false;
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        protected override void OnMousePressed(MouseButton button)
        {
            if (Enabled)
                if (button == MouseButton.Left)
                    _pressedDownByMouse = true;
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        protected override void OnMouseReleased(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                _pressedDownByMouse = false;

                // Only trigger the pressed event if the mouse was released over the control.
                // The user can move the mouse cursor away from the control while still holding
                // the mouse button down to do the well-known last-second-abort.
                if (_mouseHovering && Enabled)
                    if (!Depressed)
                        OnPressed();
            }
        }

        /// <summary>Called when a button on the gamepad has been pressed</summary>
        /// <param name="button">Button that has been pressed</param>
        /// <returns>
        ///     True if the button press was handled by the control, otherwise false.
        /// </returns>
        protected override bool OnButtonPressed(Buttons button)
        {
            if (ShortcutButton.HasValue)
                if (button == ShortcutButton.Value)
                {
                    _pressedDownByGamepadShortcut = true;
                    return true;
                }

            return false;
        }

        /// <summary>Called when a button on the gamepad has been released</summary>
        /// <param name="button">Button that has been released</param>
        protected override void OnButtonReleased(Buttons button)
        {
            if (ShortcutButton.HasValue)
                if (_pressedDownByGamepadShortcut)
                    if (button == ShortcutButton.Value)
                    {
                        _pressedDownByGamepadShortcut = false;
                        if (!Depressed)
                            OnPressed();
                    }
        }

        /// <summary>Called when a key on the keyboard has been pressed down</summary>
        /// <param name="keyCode">Code of the key that was pressed</param>
        /// <returns>
        ///     True if the key press was handled by the control, otherwise false.
        /// </returns>
        protected override bool OnKeyPressed(Keys keyCode)
        {
            if (ShortcutButton.HasValue)
                if (keyCode == KeyFromButton(ShortcutButton.Value))
                {
                    _pressedDownByKeyboardShortcut = true;
                    return true;
                }
            if (HasFocus)
                if (keyCode == Keys.Space)
                {
                    _pressedDownByKeyboard = true;
                    return true;
                }

            return false;
        }

        /// <summary>Called when a key on the keyboard has been released again</summary>
        /// <param name="keyCode">Code of the key that was released</param>
        protected override void OnKeyReleased(Keys keyCode)
        {
            if (_pressedDownByKeyboardShortcut)
                if (ShortcutButton.HasValue)
                    if (keyCode == KeyFromButton(ShortcutButton.Value))
                    {
                        _pressedDownByKeyboardShortcut = false;
                        if (!Depressed)
                            OnPressed();
                    }
            if (_pressedDownByKeyboard)
                if (keyCode == Keys.Space)
                {
                    _pressedDownByKeyboard = false;
                    if (!Depressed)
                        OnPressed();
                }
        }

        /// <summary>Called when the control is pressed</summary>
        /// <remarks>
        ///     If you were to implement a button, for example, you could trigger a 'Pressed'
        ///     event here are call a user-provided delegate, depending on your design.
        /// </remarks>
        protected virtual void OnPressed()
        {
        }

        /// <summary>Looks up the equivalent key to the gamepad button</summary>
        /// <param name="button">
        ///     Gamepad button for which the equivalent key on the keyboard will be found
        /// </param>
        /// <returns>The key that is equivalent to the specified gamepad button</returns>
        private static Keys KeyFromButton(Buttons button)
        {
            switch (button)
            {
                case Buttons.A:
                {
                    return Keys.A;
                }
                case Buttons.B:
                {
                    return Keys.B;
                }
                case Buttons.Back:
                {
                    return Keys.Back;
                }
                case Buttons.LeftShoulder:
                {
                    return Keys.L;
                }
                case Buttons.LeftStick:
                {
                    return Keys.LeftControl;
                }
                case Buttons.RightShoulder:
                {
                    return Keys.R;
                }
                case Buttons.RightStick:
                {
                    return Keys.RightControl;
                }
                case Buttons.Start:
                {
                    return Keys.Enter;
                }
                case Buttons.X:
                {
                    return Keys.X;
                }
                case Buttons.Y:
                {
                    return Keys.Y;
                }
                default:
                {
                    return Keys.None;
                }
            }
        }
    }
}