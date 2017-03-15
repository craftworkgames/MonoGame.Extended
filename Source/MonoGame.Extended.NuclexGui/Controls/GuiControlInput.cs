using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls
{
    public partial class GuiControl
    {
        /// <summary>Control the mouse was pressed down on</summary>
        private GuiControl _activatedControl;

        /// <summary>Number of game pad buttons being held down</summary>
        private int _heldButtonCount;

        /// <summary>Number of keyboard keys being held down</summary>
        private int _heldKeyCount;

        /// <summary>Mouse buttons the user is holding down over the control</summary>
        private MouseButton _heldMouseButtons;

        /// <summary>Control the mouse is currently hovering over</summary>
        private GuiControl _mouseOverControl;

        /// <summary>Whether any keys, mouse buttons or game pad buttons are beind held pressed</summary>
        private bool AnyKeysOrButtonsPressed => (_heldMouseButtons != 0) || (_heldKeyCount > 0) || (_heldButtonCount > 0);

        /// <summary>Called when a button on the game pad has been pressed</summary>
        /// <param name="button">Button that has been pressed</param>
        /// <returns>
        ///     True if the button press was processed by the control and future game pad
        ///     input belongs to the control until all buttons are released again
        /// </returns>
        internal bool ProcessButtonPress(Buttons button)
        {
            // If there's an activated control (one being held down by the mouse or having
            // accepted a previous button press), this control will get the button press
            // delivered, whether it wants to or not.
            if (_activatedControl != null)
            {
                ++_heldButtonCount;

                // If one of our children is the activated control, pass on the message
                if (_activatedControl != this)
                    _activatedControl.ProcessButtonPress(button);
                else
                    OnButtonPressed(button);

                // We're already activated, so this button press is accepted in any case
                return true;
            }

            // A button has been pressed but no control is activated currently. This means we
            // have to look for a control which feels responsible for the button press,
            // starting with ourselves.

            // Does the user code in our derived class feel responsible for this button?
            // If so, we're the new activated control and the button has been handled.
            if (OnButtonPressed(button))
            {
                _activatedControl = this;
                ++_heldButtonCount;
                return true;
            }

            // Nope, we have to ask our children to find a control that feels responsible.
            var encounteredOrderingControl = false;
            foreach (var child in _children)
            {
                // We only process one child that has the affectsOrdering field set. This
                // ensures that key presses will not be delivered to windows sitting behind
                // another window. Other siblings that are not windows are asked still, so
                // a bunch of buttons on the desktop would be asked in addition to a window.
                if (child._affectsOrdering)
                {
                    if (encounteredOrderingControl)
                        continue;

                    encounteredOrderingControl = true;
                }

                // Does this child feel responsible for the button press?
                if (child.ProcessButtonPress(button))
                {
                    _activatedControl = child;
                    ++_heldButtonCount;
                    return true;
                }
            }

            // Neither we nor any of our children felt responsible for the button. Give up.
            return false;
        }

        /// <summary>Called when a button on the game pad has been released</summary>
        /// <param name="button">Button that has been released</param>
        internal void ProcessButtonRelease(Buttons button)
        {
            // If we're the top level control, we will receive button presses and their related
            // releases even if nobody was interested in the button presses. Thus, we silently
            // ignore those presses we didn't accept.
            if (_heldButtonCount == 0)
                return;

            // If we receive a release, we must have a control on which the mouse
            // was pressed (possibly even ourselves)
            Debug.Assert(
                _activatedControl != null,
                "ProcessButtonRelease() had no control a button was pressed on; " +
                "ProcessButtonRelease() was called on a control instance, but the control " +
                "did not register a prior button press for itself or any of its child controls"
            );

            --_heldButtonCount;
            if (_activatedControl != this)
                _activatedControl.ProcessButtonRelease(button);
            else
                OnButtonReleased(button);

            // If no more keys buttons are being held down, clear the activated control
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>
        ///     Called when the mouse has left the control and is no longer hovering over it
        /// </summary>
        internal void ProcessMouseLeave()
        {
            // Because the mouse has left us, if we have a mouse-over control, it also
            // cannot be over one of our children Children leaving the parent container
            // are not supported by design and for consistency, the behavior is tweaked
            // so the children are left when the parent is left - this avoids strange
            // behavior like being able to select a control if entering it with the mouse
            // from the container side but being unable to select it if entering from
            // the outside.
            if (_mouseOverControl != null)
            {
                if (_mouseOverControl != this)
                    _mouseOverControl.ProcessMouseLeave();
                else
                    OnMouseLeft();

                _mouseOverControl = null;
            }
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        /// <returns>Whether the control has processed the mouse press</returns>
        internal bool ProcessMousePress(MouseButton button)
        {
            // We remember the control the mouse was pressed over and won't replace it for
            // as long as the mouse is being held down. This ensures the mouse release
            // notification is always delivered to a control, even if the mouse is released
            // after moving it away from the control.
            if (_activatedControl == null)
            {
                _activatedControl = _mouseOverControl;

                // If we received an initial mouse press outside of our control area,
                // someone is feeding us notifications we shouldn't be receiving. The best
                // thing we can do is ignore this notification. This is a normal situation
                // for the top level control which does the input filtering.
                if (_activatedControl == null)
                    return false;

                // If we're a control that can appear on top of or below our siblings in
                // the z order, bring us into foreground since the user just clicked on us.
                if (_activatedControl != this)
                {
                    if (_activatedControl._affectsOrdering)
                        _children.MoveToStart(_children.IndexOf(_activatedControl));
                }
            }

            // Add the buttons to the list of mouse buttons being held down. This is used
            // to track when we should clear the mouse-over control again.
            _heldMouseButtons |= button;

            // If the mouse is over another control, pass on the mouse press.
            if (_activatedControl != this)
                return _activatedControl.ProcessMousePress(button);
            // Otherwise, the mouse press applies to us

            // If this control can take the input focus, make it the focused control
            if (_screen != null)
            {
                var focusable = this as IFocusable;
                if ((focusable != null) && focusable.CanGetFocus)
                    _screen.FocusedControl = this;
            }

            // Deliver the notification to the control deriving from us
            OnMousePressed(button);
            return true;
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        internal void ProcessMouseRelease(MouseButton button)
        {
            // When the mouse is clicked on game window's border and the user drags it
            // into the GUI area, we will get a rogue mouse release message without
            // the related mouse press. We ignore such rogue mouse release messages.
            if ((_heldMouseButtons & button) != button)
                return;

            // If we receive a release, we must have a control on which the mouse
            // was pressed (possibly even ourselves)
            Debug.Assert(
                _activatedControl != null,
                "ProcessMouseRelease() had no control the mouse was pressed on; " +
                "ProcessMouseRelease() was called on a control instance, but the control " +
                "did not register a prior mouse press over itself or any of its child controls"
            );

            // Remove the button from the list of mouse buttons being held down. This
            // allows us to see when we can clear the mouse-press control.
            _heldMouseButtons &= ~button;

            // If the mouse was held over one of our childs, pass on the notification
            if (_activatedControl != this)
                _activatedControl.ProcessMouseRelease(button);
            else
                OnMouseReleased(button);

            // If no more mouse buttons are being held down, clear the mouse-press control
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>Processes mouse movement notifications</summary>
        /// <param name="containerWidth">Absolute width of the control's container</param>
        /// <param name="containerHeight">Absolute height of the control's container</param>
        /// <param name="x">Absolute X position of the mouse within the container</param>
        /// <param name="y">Absolute Y position of the mouse within the container</param>
        internal void ProcessMouseMove(float containerWidth, float containerHeight, float x, float y)
        {
            // Calculate the absolute pixel position and size of this control
            var size = Bounds.Size.ToOffset(containerWidth, containerHeight);

            // If a mouse button is being held down, the mouse movement notification is
            // delivered to the control the mouse was pressed on first. This guarantees that
            // windows can be dragged even if the mouse was close to the window border and
            // leaves the window during dragging.
            if (_activatedControl != null)
            {
                var mouseX = x - Bounds.Location.X.ToOffset(containerWidth);
                var mouseY = y - Bounds.Location.Y.ToOffset(containerHeight);

                // Deliver the mouse move notifcation (either to our own user code or
                // to the control the mouse of hovering over)
                if (_activatedControl != this)
                    _activatedControl.ProcessMouseMove(size.X, size.Y, mouseX, mouseY);
                else
                    OnMouseMoved(mouseX, mouseY);
            }

            // Calculate the absolute mouse position. We cannot reuse the value calculated
            // in the mouse-press handling code because the control could have been moved when
            // we called OnMouseMoved() - a typical use case for draggable controls.
            x -= Bounds.Location.X.ToOffset(containerWidth);
            y -= Bounds.Location.Y.ToOffset(containerHeight);

            // Check whether the mouse is hovering over one of our children and if so,
            // pass on the mouse movement notification to the child.
            foreach (var control in _children)
            {
                var childBounds = control.Bounds.ToOffset(size.X, size.Y);

                // Is the mouse over this child?
                if (childBounds.Contains(new Point2(x, y)))
                {
                    SwitchMouseOverControl(control);

                    // Hand over the mouse movement data to the child control the mouse is
                    // hovering over. If this is the mouse-press control, do nothing because
                    // we already delivered the movement notification out of order.
                    if (_mouseOverControl != _activatedControl)
                        _mouseOverControl.ProcessMouseMove(size.X, size.Y, x, y);

                    // We got our mouse-over control, end processing.
                    return;
                }
            }

            // The mouse was over none of our children, so it must be hovering over us,
            // unless we're the control being pressed down, in which case we'd also be
            // getting mouse movement data outside of our boundaries. In this case, we
            // only should become the mouse-over control is actually over us.
            if (
                (x >= 0.0f) && (x < size.X) &&
                (y >= 0.0f) && (y < size.Y)
            )
            {
                SwitchMouseOverControl(this);

                // If we weren't pressed, we didn't deliver the out-of-order update to
                // our implementation. Send our implementation a normal ordered update.
                if (_activatedControl == null)
                    OnMouseMoved(x, y);
            }
            else
            {
                // redundant - our parent handles this - but convenient for unit tests
                ProcessMouseLeave();
            }
        }

        /// <summary>Called when the mouse wheel has been rotated</summary>
        /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
        internal void ProcessMouseWheel(float ticks)
        {
            // If the mouse is being held down on a control, give it any mouse wheel
            // messages. This enables some exotic uses for the mouse wheel, such as holding
            // an object with the mouse button and scaling it with the wheel at the same time.
            if (_activatedControl != null)
            {
                if (_activatedControl != this)
                {
                    _activatedControl.ProcessMouseWheel(ticks);
                    return;
                }
            }

            // If the mouse wheel has been used normally, send the wheel notifications to
            // the control the mouse is over.
            if (_mouseOverControl != null)
            {
                if (_mouseOverControl != this)
                {
                    _mouseOverControl.ProcessMouseWheel(ticks);
                    return;
                }
            }

            // We're the control the mouse is over, let the user code handle
            // the mouse wheel rotation
            OnMouseWheel(ticks);
        }

        /// <summary>Called when a key on the keyboard has been pressed down</summary>
        /// <param name="keyCode">Code of the key that was pressed</param>
        /// <param name="repetition">Whether the key press is due to the user holding down a key</param>
        internal bool ProcessKeyPress(Keys keyCode, bool repetition)
        {
            // If there's an activated control (one being held down by the mouse or having
            // accepted a previous key press), this control will get the key press delivered,
            // whether it wants to or not. We don't want to track for each key which control
            // is currently processing it. ;-)
            if (_activatedControl != null)
            {
                if (!repetition)
                    ++_heldKeyCount;

                // If one of our children is the activated control, pass on the message
                if (_activatedControl != this)
                    _activatedControl.ProcessKeyPress(keyCode, repetition);
                else
                    OnKeyPressed(keyCode); // We're the activated control

                return true; // Ignore user code and always accept the key press
            }

            // A key has been pressed but no control is activated currently. This means we
            // have to look for a control which feels responsible for the key press, starting
            // with ourselves.

            // Does the user code in our derived class feel responsible for this key?
            // If so, we're the new activated control and the key has been handled.
            if (OnKeyPressed(keyCode))
            {
                _activatedControl = this;
                ++_heldKeyCount;
                return true;
            }

            // Nope, we have to ask our children (and they, potentially recursively, theirs)
            // to find a control that feels responsible.
            var encounteredOrderingControl = false;
            for (var index = 0; index < _children.Count; ++index)
            {
                var child = _children[index];

                // We only process one child that has the affectsOrdering field set. This
                // ensures that key presses will not be delivered to windows sitting behind
                // another window. Other siblings that are not windows are asked still.
                if (child._affectsOrdering)
                {
                    if (encounteredOrderingControl)
                        continue;
                    else
                        encounteredOrderingControl = true;
                }

                // Does this child feel responsible for the key press?
                if (child.ProcessKeyPress(keyCode, repetition))
                {
                    _activatedControl = child;
                    ++_heldKeyCount;
                    return true;
                }
            }

            // Neither we nor any of our children felt responsible for the key. Give up.
            return false;
        }

        /// <summary>Called when a key on the keyboard has been released again</summary>
        /// <param name="keyCode">Code of the key that was released</param>
        internal void ProcessKeyRelease(Keys keyCode)
        {
            // Any key release should have an associated key press, otherwise, someone
            // delivered notifications to us we should not have received.
            Debug.Assert(
                _heldKeyCount > 0,
                "ProcessKeyRelease() called more often then ProcessKeyPress(); " +
                "ProcessKeyRelease() was called more often the ProcessKeyPress() has been " +
                "called with the repetition parameter set to false"
            );

            // If we receive a release, we must have a control on which the mouse
            // was pressed (possibly even ourselves)
            Debug.Assert(
                _activatedControl != null,
                "ProcessKeyRelease() had no control a key was pressed on; " +
                "ProcessKeyRelease() was called on a control instance, but the control " +
                "did not register a prior key press for itself or any of its child controls"
            );

            --_heldKeyCount;
            if (_activatedControl != this)
                _activatedControl.ProcessKeyRelease(keyCode);
            else
                OnKeyReleased(keyCode);

            // If no more keys buttons are being held down, clear the activated control
            if (!AnyKeysOrButtonsPressed)
                _activatedControl = null;
        }

        /// <summary>Switches the mouse over control to a different control</summary>
        /// <param name="newMouseOverControl">New control the mouse is hovering over</param>
        private void SwitchMouseOverControl(GuiControl newMouseOverControl)
        {
            if (_mouseOverControl != newMouseOverControl)
            {
                // Tell the previous mouse-over control that the mouse is no longer
                // hovering over it
                if (_mouseOverControl != null)
                    _mouseOverControl.ProcessMouseLeave();

                _mouseOverControl = newMouseOverControl;

                // Inform the new mouse-over control that the mouse is now over it
                newMouseOverControl.OnMouseEntered();
            }
        }
    }
}