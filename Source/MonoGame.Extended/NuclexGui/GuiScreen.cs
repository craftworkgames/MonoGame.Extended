using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.NuclexGui.Controls;
using MonoGame.Extended.NuclexGui.Input;
using MonoGame.Extended.NuclexGui.Support;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui
{
    /// <summary>Manages the controls and their state on a GUI screen</summary>
    /// <remarks>
    ///     This class manages the global state of a distinct user interface. Unlike your
    ///     typical GUI library, the Nuclex.UserInterface library can handle any number of
    ///     simultaneously active user interfaces at the same time, making the library
    ///     suitable for usage on virtual ingame computers and multi-client environments
    ///     such as split-screen games or switchable graphical terminals.
    /// </remarks>
    public class GuiScreen : IInputReceiver
    {
        /// <summary>Highest value in the Keys enumeration</summary>
        private static readonly int _maxKeyboardKey =
            (int) EnumHelper.GetHighestValue<Keys>();

        /// <summary>Control responsible for hosting the GUI's top-level controls</summary>
        private readonly GuiDesktopControl _desktopControl;

        /// <summary>Child that currently has the input focus</summary>
        /// <remarks>
        ///     If this field is non-null, all keyboard input sent to the Gui is handed
        ///     over to the focused control. Otherwise, keyboard input is discarded.
        /// </remarks>
        private readonly Support.WeakReference<GuiControl> _focusedControl;

        /// <summary>Keys on the keyboard the user is currently holding down</summary>
        private readonly BitArray _heldKeys;

        /// <summary>Control the user has activated through one of the input devices</summary>
        private GuiControl _activatedControl;

        /// <summary>Buttons on the game pad the user is currently holding down</summary>
        private Buttons _heldButtons;

        /// <summary>Number of keys being held down on the keyboard</summary>
        private int _heldKeyCount;

        /// <summary>Mouse buttons currently being held down</summary>
        private MouseButton _heldMouseButtons;

        /// <summary>Size of the GUI area in world units or pixels</summary>
        private Vector2 _size;

        /// <summary>Initializes a new GUI</summary>
        public GuiScreen() : this(0, 0)
        {
        }

        /// <summary>Initializes a new GUI</summary>
        /// <param name="width">Width of the area the GUI can occupy</param>
        /// <param name="height">Height of the area the GUI can occupy</param>
        /// <remarks>
        ///     Width and height should reflect the entire drawable area of your GUI. If you
        ///     want to limit the region which the GUI is allowed to use (eg. to only use the
        ///     safe area of a TV) please resize the desktop control accordingly!
        /// </remarks>
        public GuiScreen(float width, float height)
        {
            Width = width;
            Height = height;

            _heldKeys = new BitArray(_maxKeyboardKey + 1);
            _heldButtons = 0;

            // By default, the desktop control will cover the whole drawing area
            _desktopControl = new GuiDesktopControl
            {
                Bounds = new UniRectangle(new UniVector(0, 0), new UniVector(1, 1))
            };

            _desktopControl.SetScreen(this);

            _focusedControl = new Support.WeakReference<GuiControl>(null);
        }

        /// <summary>Width of the screen in pixels</summary>
        public float Width
        {
            get { return _size.X; }
            set { _size.X = value; }
        }

        /// <summary>Height of the screen in pixels</summary>
        public float Height
        {
            get { return _size.Y; }
            set { _size.Y = value; }
        }

        /// <summary>Control responsible for hosting the GUI's top-level controls</summary>
        public GuiControl Desktop => _desktopControl;

        /// <summary>
        ///     Whether any keys, mouse buttons or game pad buttons are beind held pressed
        /// </summary>
        private bool AnyKeysOrButtonsPressed => (_heldMouseButtons != 0) ||
                                                (_heldKeyCount > 0) ||
                                                (_heldButtons != 0);

        /// <summary>Whether the GUI has currently captured the input devices</summary>
        /// <remarks>
        ///     <para>
        ///         When you mix GUIs and gameplay (for example, in a strategy game where the GUI
        ///         manages the build menu and the remainder of the screen belongs to the game),
        ///         it is important to keep control of who currently owns the input devices.
        ///     </para>
        ///     <para>
        ///         Assume the player is drawing a selection rectangle around some units using
        ///         the mouse. He will press the mouse button outside any GUI elements, keep
        ///         holding it down and possibly drag over the GUI. Until the player lets go
        ///         of the mouse button, input exclusively belongs to the game. The same goes
        ///         vice versa, of course.
        ///     </para>
        ///     <para>
        ///         This property tells whether the GUI currently thinks that all input belongs
        ///         to it. If it is true, the game should not process any input. The GUI will
        ///         implement the input model as described here and respect the game's ownership
        ///         of the input devices if a mouse button is pressed outside of the GUI. To
        ///         correctly handle input device ownership, send all input to the GUI
        ///         regardless of this property's value, then check this property and if it
        ///         returns false let your game process the input.
        ///     </para>
        /// </remarks>
        public bool IsInputCaptured => _desktopControl.IsInputCaptured;

        /// <summary>True if the mouse is currently hovering over any GUI elements</summary>
        /// <remarks>
        ///     Useful if you mix gameplay with a GUI and use different mouse cursors
        ///     depending on the location of the mouse. As long as input is not captured
        ///     (see <see cref="IsInputCaptured" />) you can use this property to know
        ///     whether you should use the standard GUI mouse cursor or let your game
        ///     decide which cursor to use.
        /// </remarks>
        public bool IsMouseOverGui => _desktopControl.IsMouseOverGui;

        /// <summary>Child control that currently has the input focus</summary>
        public GuiControl FocusedControl
        {
            get
            {
                var current = _focusedControl.Target;
                if ((current != null) && ReferenceEquals(current.Screen, this))
                    return current;
                return null;
            }
            set
            {
                var current = _focusedControl.Target;
                if (!ReferenceEquals(value, current))
                {
                    _focusedControl.Target = value;
                    OnFocusChanged(value);
                }
            }
        }

        /// <summary>Injects a command into the processor</summary>
        /// <param name="command">Input command that will be injected</param>
        public void InjectCommand(Command command)
        {
            switch (command)
            {
                // Accept or cancel the current control
                case Command.Accept:
                case Command.Cancel:
                {
                    var focusedControl = FocusedControl;
                    if (focusedControl == null)
                        return; // Also catches when focusedControl is not part of the tree

                    // TODO: Should this be propagated down the control tree?
                    focusedControl.ProcessCommand(command);

                    break;
                }

                // Change focus to another control
                case Command.SelectPrevious:
                case Command.SelectNext:
                {
                    // TODO: Implement focus switching

                    break;
                }

                // Control specific. Changes focus if unhandled.
                case Command.Up:
                case Command.Down:
                case Command.Left:
                case Command.Right:
                {
                    var focusedControl = FocusedControl;
                    if (focusedControl == null)
                        return; // Also catches when focusedControl is not part of the tree

                    // First send the command to the focused control. If the control handles
                    // the command, there's nothing for us to do. Otherwise, use the directional
                    // commands for focus switching.
                    if (focusedControl.ProcessCommand(command))
                        return;

                    // These will be determined in the following code block
                    var nearestDistance = float.NaN;
                    GuiControl nearestControl = null;
                    {
                        // Determine the center of the focused control
                        var parentBounds = focusedControl.Parent.GetAbsoluteBounds();
                        var focusedBounds = focusedControl.Bounds.ToOffset(
                            parentBounds.Width, parentBounds.Height
                        );

                        // Search all siblings of the focused control for the nearest control in the
                        // direction the command asks to move into
                        var siblings = focusedControl.Parent.Children;
                        foreach (var sibling in siblings)
                        {
                            // Only consider this sibling if it's focusable
                            if (!ReferenceEquals(sibling, focusedControl) && CanControlGetFocus(sibling))
                            {
                                var siblingBounds = sibling.Bounds.ToOffset(
                                    parentBounds.Width, parentBounds.Height
                                );

                                // Calculate the distance the control has in the direction focus is being
                                // changed to. If the control doesn't lie in that direction, NaN will
                                // be returned
                                var distance = GetDirectionalDistance(
                                    ref focusedBounds, ref siblingBounds, command
                                );
                                if (float.IsNaN(nearestDistance) || (distance < nearestDistance))
                                {
                                    nearestControl = sibling;
                                    nearestDistance = distance;
                                }
                            }
                        }
                    } // beauty scope

                    // Search completed, if we found a candidate, change focus to it
                    if (!float.IsNaN(nearestDistance))
                        FocusedControl = nearestControl;

                    break;
                }
            }
        }

        /// <summary>Called when a key on the keyboard has been pressed down</summary>
        /// <param name="keyCode">Code of the key that was pressed</param>
        public void InjectKeyPress(Keys keyCode)
        {
            var repetition = _heldKeys.Get((int) keyCode);

            // If a control is activated, it will receive any input notifications
            if (_activatedControl != null)
            {
                _activatedControl.ProcessKeyPress(keyCode, repetition);
                if (!repetition)
                {
                    ++_heldKeyCount;
                    _heldKeys.Set((int) keyCode, true);
                }
                return;
            }

            // No control is activated, try the focused control before searching
            // the entire tree for a responder.
            var focusedControl = _focusedControl.Target;
            if (focusedControl != null)
                if (focusedControl.ProcessKeyPress(keyCode, false))
                {
                    _activatedControl = focusedControl;
                    if (!repetition)
                    {
                        ++_heldKeyCount;
                        _heldKeys.Set((int) keyCode, true);
                    }
                    return;
                }

            // Focused control didn't process the notification, now let the desktop
            // control traverse the entire control tree is earch for a handler.
            if (_desktopControl.ProcessKeyPress(keyCode, false))
            {
                _activatedControl = _desktopControl;
                if (!repetition)
                {
                    ++_heldKeyCount;
                    _heldKeys.Set((int) keyCode, true);
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Up:
                    {
                        InjectCommand(Command.Up);
                        break;
                    }
                    case Keys.Down:
                    {
                        InjectCommand(Command.Down);
                        break;
                    }
                    case Keys.Left:
                    {
                        InjectCommand(Command.Left);
                        break;
                    }
                    case Keys.Right:
                    {
                        InjectCommand(Command.Right);
                        break;
                    }
                    case Keys.Enter:
                    {
                        InjectCommand(Command.Accept);
                        break;
                    }
                    case Keys.Escape:
                    {
                        InjectCommand(Command.Cancel);
                        break;
                    }
                }
            }
        }

        /// <summary>Called when a key on the keyboard has been released again</summary>
        /// <param name="keyCode">Code of the key that was released</param>
        public void InjectKeyRelease(Keys keyCode)
        {
            if (!_heldKeys.Get((int) keyCode))
                return;
            --_heldKeyCount;
            _heldKeys.Set((int) keyCode, false);

            // If a control signed responsible for the earlier key press, it will now
            // receive the release notification.
            _activatedControl?.ProcessKeyRelease(keyCode);

            // Reset the activated control if the user has released all buttons on all
            // input devices.
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>Handle user text input by a physical or virtual keyboard</summary>
        /// <param name="character">Character that has been entered</param>
        public void InjectCharacter(char character)
        {
            // Send the text to the currently focused control in the GUI
            var focusedControl = _focusedControl.Target;
            var writable = focusedControl as IWritable;
            writable?.OnCharacterEntered(character);
        }

        /// <summary>Called when a button on the gamepad has been pressed</summary>
        /// <param name="button">Button that has been pressed</param>
        public void InjectButtonPress(Buttons button)
        {
            var newHeldButtons = _heldButtons | button;
            if (newHeldButtons == _heldButtons)
                return;
            _heldButtons = newHeldButtons;

            // If a control is activated, it will receive any input notifications
            if (_activatedControl != null)
            {
                _activatedControl.ProcessButtonPress(button);
                return;
            }

            // No control is activated, try the focused control before searching
            // the entire tree for a responder.
            var focusedControl = _focusedControl.Target;
            if (focusedControl != null)
                if (focusedControl.ProcessButtonPress(button))
                {
                    _activatedControl = focusedControl;
                    return;
                }

            // Focused control didn't process the notification, now let the desktop
            // control traverse the entire control tree is earch for a handler.
            if (_desktopControl.ProcessButtonPress(button))
                _activatedControl = _desktopControl;
        }

        /// <summary>Called when a button on the gamepad has been released</summary>
        /// <param name="button">Button that has been released</param>
        public void InjectButtonRelease(Buttons button)
        {
            if ((_heldButtons & button) == 0)
                return;
            _heldButtons &= ~button;

            // If a control signed responsible for the earlier button press, it will now
            // receive the release notification.
            _activatedControl?.ProcessButtonRelease(button);

            // Reset the activated control if the user has released all buttons on all
            // input devices.
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>Injects a mouse position update into the GUI</summary>
        /// <param name="x">X coordinate of the mouse cursor within the screen</param>
        /// <param name="y">Y coordinate of the mouse cursor within the screen</param>
        public void InjectMouseMove(float x, float y)
        {
            _desktopControl.ProcessMouseMove(_size.X, _size.Y, x, y);
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        public void InjectMousePress(MouseButton button)
        {
            _heldMouseButtons |= button;

            // If a control is activated, it will receive any input notifications
            if (_activatedControl != null)
            {
                _activatedControl.ProcessMousePress(button);
                return;
            }

            // No control was activated, so the desktop control becomes activated and
            // is responsible for routing the input to the control under the mouse.
            _activatedControl = _desktopControl;
            _desktopControl.ProcessMousePress(button);
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        public void InjectMouseRelease(MouseButton button)
        {
            _heldMouseButtons &= ~button;

            // If a control signed responsible for the earlier mouse press, it will now
            // receive the release notification.
            _activatedControl?.ProcessMouseRelease(button);

            // Reset the activated control if the user has released all buttons on all
            // input devices.
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>Called when the mouse wheel has been rotated</summary>
        /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
        public void InjectMouseWheel(float ticks)
        {
            if (_activatedControl != null)
                _activatedControl.ProcessMouseWheel(ticks);
            else
                _desktopControl.ProcessMouseWheel(ticks);
        }

        /// <summary>Triggered when the control in focus changes</summary>
        public event EventHandler<ControlEventArgs> FocusChanged;

        /// <summary>Triggers the FocusChanged event</summary>
        /// <param name="focusedControl">Control that has gotten the input focus</param>
        private void OnFocusChanged(GuiControl focusedControl)
        {
            FocusChanged?.Invoke(this, new ControlEventArgs(focusedControl));
        }

        /// <summary>
        ///     Determines the distance of one rectangle to the other, also taking direction
        ///     into account
        /// </summary>
        /// <param name="ownBounds">Boundaries of the base rectangle</param>
        /// <param name="otherBounds">Boundaries of the other rectangle</param>
        /// <param name="direction">Direction into which distance will be determined</param>
        /// <returns>
        ///     The direction of the other rectangle of NaN if it didn't lie in that direction
        /// </returns>
        private static float GetDirectionalDistance(
            ref RectangleF ownBounds, ref RectangleF otherBounds, Command direction
        )
        {
            float closestPointX, closestPointY;
            float distance;

            var isVertical =
                (direction == Command.Up) ||
                (direction == Command.Down);

            if (isVertical)
            {
                var ownCenterX = ownBounds.X + ownBounds.Width/2.0f;

                // Take an imaginary line through the other control's center, perpendicular
                // to the specified direction. Then locate the closest point on that line
                // to our own center.
                closestPointX = Math.Min(Math.Max(ownCenterX, otherBounds.Left), otherBounds.Right);
                closestPointY = otherBounds.Y + otherBounds.Height/2.0f;

                // Find out whether we need to check the diagonal quadrant boundary
                var leavesLeft = closestPointX < ownBounds.Left;
                var leavesRight = closestPointX > ownBounds.Right;

                // 
                float sideY;
                if (direction == Command.Up)
                {
                    sideY = ownBounds.Top;
                    if ((closestPointY > sideY) && (leavesLeft || leavesRight))
                        return float.NaN;
                    distance = sideY - closestPointY;
                }
                else
                {
                    sideY = ownBounds.Bottom;
                    if ((closestPointY < sideY) && (leavesLeft || leavesRight))
                        return float.NaN;
                    distance = closestPointY - sideY;
                }

                var distanceY = Math.Abs(sideY - closestPointY);
                if (leavesLeft)
                {
                    var distanceX = Math.Abs(ownBounds.Left - closestPointX);
                    if (distanceX > distanceY)
                        return float.NaN;
                }
                else if (leavesRight)
                {
                    var distanceX = Math.Abs(closestPointX - ownBounds.Right);
                    if (distanceX > distanceY)
                        return float.NaN;
                }
            }
            else
            {
                var ownCenterY = ownBounds.Y + ownBounds.Height/2.0f;

                // Take an imaginary line through the other control's center, perpendicular
                // to the specified direction. Then locate the closest point on that line
                // to our own center.
                closestPointX = otherBounds.X + otherBounds.Width/2.0f;
                closestPointY = Math.Min(Math.Max(ownCenterY, otherBounds.Top), otherBounds.Bottom);

                // Find out whether we need to check the diagonal quadrant boundary
                var leavesTop = closestPointY < ownBounds.Top;
                var leavesBottom = closestPointY > ownBounds.Bottom;

                float sideX;
                if (direction == Command.Left)
                {
                    sideX = ownBounds.Left;
                    if ((closestPointX > sideX) && (leavesTop || leavesBottom))
                        return float.NaN;
                    distance = sideX - closestPointX;
                }
                else
                {
                    sideX = ownBounds.Right;
                    if ((closestPointX < sideX) && (leavesTop || leavesBottom))
                        return float.NaN;
                    distance = closestPointX - sideX;
                }

                var distanceX = Math.Abs(sideX - closestPointX);
                if (leavesTop)
                {
                    var distanceY = Math.Abs(ownBounds.Top - closestPointY);
                    if (distanceY > distanceX)
                        return float.NaN;
                }
                else if (leavesBottom)
                {
                    var distanceY = Math.Abs(closestPointY - ownBounds.Bottom);
                    if (distanceY > distanceX)
                        return float.NaN;
                }
            }

            return distance < 0.0f ? float.NaN : distance;
        }

        /// <summary>Determines whether a control can obtain the input focus</summary>
        /// <param name="control">Control that will be checked for focusability</param>
        /// <returns>True if the specified control can obtain the input focus</returns>
        private static bool CanControlGetFocus(GuiControl control)
        {
            var focusableControl = control as IFocusable;
            if (focusableControl != null)
                return focusableControl.CanGetFocus;
            return false;
        }
    }
}