using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Control through which the user can enter text</summary>
    /// <remarks>
    ///     <para>
    ///         Through this control, users can be asked to enter an arbitrary string
    ///         of characters, their name for example. Desktop users can enter text through
    ///         their normal keyboard where Windows' own key translation is used to
    ///         support regional settings and custom keyboard layouts.
    ///     </para>
    ///     <para>
    ///         XBox 360 users will open the virtual keyboard when the input box gets
    ///         the input focus and can add characters by selecting them from the virtual
    ///         keyboard's character matrix.
    ///     </para>
    /// </remarks>
    public class GuiInputControl : GuiControl, IWritable
    {
        /// <summary>Position of the cursor within the text</summary>
        private int _caretPosition;

        /// <summary>Tick count at the time the caret was last moved</summary>
        private int _lastCaretMovementTicks;

        /// <summary>X coordinate of the last known mouse position</summary>
        private float _mouseX;

        /// <summary>Y coordinate of the last known mouse position</summary>
        private float _mouseY;

        /// <summary>Array used to store characters before they are appended</summary>
        private readonly char[ /*1*/] _singleCharArray;

        /// <summary>Text the user has entered into the text input control</summary>
        private readonly StringBuilder _text;

        /// <summary>Whether user interaction with the control is allowed</summary>
        public bool Enabled;

        /// <summary>Description to be displayed in the on-screen keyboard</summary>
        public string GuideDescription;

        /// <summary>Title to be displayed in the on-screen keyboard</summary>
        public string GuideTitle;

        /// <summary>
        ///     Can be set by renderers to enable cursor positioning by the mouse
        /// </summary>
        public IOpeningLocator OpeningLocator;

        /// <summary>Initializes a new text input control</summary>
        public GuiInputControl()
        {
            _singleCharArray = new char[1];
            _text = new StringBuilder(64);

            Enabled = true;
            GuideTitle = "Text Entry";
            GuideDescription = "Please enter the text for this input field";
        }

        /// <summary>Position of the cursor within the text</summary>
        public int CaretPosition
        {
            get { return _caretPosition; }
            set
            {
                if ((value < 0) || (value > Text.Length))
                    throw new ArgumentException("Invalid caret position", "CaretPosition");

                _caretPosition = value;
            }
        }

        /// <summary>Whether the control currently has the input focus</summary>
        public bool HasFocus => (Screen != null) &&
                                ReferenceEquals(Screen.FocusedControl, this);

        /// <summary>Elapsed milliseconds since the user last moved the caret</summary>
        /// <remarks>
        ///     This is an unusual property for an input box to have. It is retrieved by
        ///     the renderer and could be used for several purposes, such as lighting up
        ///     a control when text is entered to provide better visual tracking or
        ///     preventing the cursor from blinking whilst the user is typing.
        /// </remarks>
        public int MillisecondsSinceLastCaretMovement => Environment.TickCount - _lastCaretMovementTicks;

        /// <summary>Text that is being displayed on the control</summary>
        public string Text
        {
            get { return _text.ToString(); }
            set
            {
                _text.Remove(0, _text.Length);
                _text.Append(value);

                // Cursor index is in openings between letters, including before first
                // and after last letter, so text.Length is a valid position.
                if (_caretPosition > _text.Length)
                    _caretPosition = _text.Length;
            }
        }

        /// <summary>Called when the user has entered a character</summary>
        /// <param name="character">Character that has been entered</param>
        void IWritable.OnCharacterEntered(char character)
        {
            OnCharacterEntered(character);
        }

        /// <summary>Whether the control can currently obtain the input focus</summary>
        bool IFocusable.CanGetFocus => Enabled;

        /// <summary>Title to be displayed in the on-screen keyboard</summary>
        string IWritable.GuideTitle => GuideTitle;

        /// <summary>Description to be displayed in the on-screen keyboard</summary>
        string IWritable.GuideDescription => GuideDescription;

        /// <summary>Called when the user has entered a character</summary>
        /// <param name="character">Character that has been entered</param>
        protected virtual void OnCharacterEntered(char character)
        {
            // For some reason, Windows translates Backspace to a character :)
            if (character != '\b')
            {
                UpdateLastCaretMovementTicks();

                // There's no single-character overload on the XBox 360...
                _singleCharArray[0] = character;
                _text.Insert(_caretPosition, _singleCharArray);
                ++_caretPosition;
            }
        }

        /// <summary>Called when a key on the keyboard has been pressed down</summary>
        /// <param name="keyCode">Code of the key that was pressed</param>
        /// <returns>
        ///     True if the key press was handles by the control, otherwise false.
        /// </returns>
        /// <remarks>
        ///     If the control indicates that it didn't handle the key press, it will not
        ///     receive the associated key release notification.
        /// </remarks>
        protected override bool OnKeyPressed(Keys keyCode)
        {
            // We only accept keys if we have the focus. If the notification is sent in search
            // for a key handler without the input box being focused, we will not respond to
            // the key press in order to not sabotage shortcut keys for other controls.
            if (!HasFocus)
                return false;

            switch (keyCode)
            {
                // Backspace: erase the character left of the caret
                case Keys.Back:
                {
                    if (_caretPosition > 0)
                    {
                        UpdateLastCaretMovementTicks();

                        _text.Remove(_caretPosition - 1, 1);
                        --_caretPosition;
                    }
                    break;
                }

                // Delete: erase the character right of the caret
                case Keys.Delete:
                {
                    if (_caretPosition < _text.Length)
                    {
                        UpdateLastCaretMovementTicks();

                        _text.Remove(_caretPosition, 1);
                    }
                    break;
                }

                // Cursor left: move the caret to the left by one character
                case Keys.Left:
                {
                    if (_caretPosition > 0)
                    {
                        UpdateLastCaretMovementTicks();

                        --_caretPosition;
                    }
                    break;
                }

                // Cursor right: move the caret to the right by one character
                case Keys.Right:
                {
                    if (_caretPosition < _text.Length)
                    {
                        UpdateLastCaretMovementTicks();

                        ++_caretPosition;
                    }
                    break;
                }

                // Home: place the caret before the first character
                case Keys.Home:
                {
                    UpdateLastCaretMovementTicks();
                    _caretPosition = 0;
                    break;
                }

                // Home: place the caret behind the last character
                case Keys.End:
                {
                    UpdateLastCaretMovementTicks();
                    _caretPosition = _text.Length;
                    break;
                }

                // Keys that can be used to navigate the dialog
                case Keys.Tab:
                case Keys.Up:
                case Keys.Down:
                case Keys.Enter:
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Called when the mouse position is updated</summary>
        /// <param name="x">X coordinate of the mouse cursor on the control</param>
        /// <param name="y">Y coordinate of the mouse cursor on the control</param>
        protected override void OnMouseMoved(float x, float y)
        {
            _mouseX = x;
            _mouseY = y;
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        protected override void OnMousePressed(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                if (OpeningLocator != null)
                {
                    var absoluteBounds = GetAbsoluteBounds();
                    var absolutePosition = new Vector2(absoluteBounds.X + _mouseX, absoluteBounds.Y + _mouseY);
                    _caretPosition = OpeningLocator.GetClosestOpening(absoluteBounds, Text, absolutePosition);
                }
                else
                {
                    // Nope, our renderer is being secretive
                    MoveCaretToEnd();
                }
            }
        }

        /// <summary>Handles user text input by a physical keyboard</summary>
        /// <param name="character">Character that has been entered</param>
        internal void ProcessCharacter(char character)
        {
            // This notifications always concerns ourselves because it is only sent
            // to the focused control
            OnCharacterEntered(character);
        }

        /// <summary>Moves the caret to the end of the text</summary>
        private void MoveCaretToEnd()
        {
            UpdateLastCaretMovementTicks();
            _caretPosition = _text.Length;
        }

        /// <summary>Updates the tick count when the caret was last moved</summary>
        /// <remarks>
        ///     Used to prevent the caret from blinking when
        /// </remarks>
        private void UpdateLastCaretMovementTicks()
        {
            _lastCaretMovementTicks = Environment.TickCount;
        }
    }
}