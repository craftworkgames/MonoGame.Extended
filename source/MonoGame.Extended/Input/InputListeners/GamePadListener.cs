using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input.InputListeners
{
    /// <summary>
    ///     This is a listener that exposes several events for easier handling of gamepads.
    /// </summary>
    public class GamePadListener : InputListener
    {
        private static readonly bool[] _gamePadConnections = new bool[4];

        // These buttons are not to be evaluated normally, but with the debounce filter
        // in their respective methods.
        private readonly Buttons[] _excludedButtons =
        {
            Buttons.LeftTrigger, Buttons.RightTrigger,
            Buttons.LeftThumbstickDown, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickRight,
            Buttons.LeftThumbstickLeft,
            Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp,
            Buttons.RightThumbstickDown
        };

        private GamePadState _currentState;
        //private int _lastPacketNumber;
        // Implementation doesn't work, see explanation in CheckAllButtons().
        private GameTime _gameTime;
        private Buttons _lastButton;
        private Buttons _lastLeftStickDirection;
        private Buttons _lastRightStickDirection;
        private GamePadState _lastThumbStickState;

        private GamePadState _lastTriggerState;

        private float _leftCurVibrationStrength;
        private bool _leftStickDown;
        private bool _leftTriggerDown;
        private bool _leftVibrating;
        private GameTime _previousGameTime;
        private GamePadState _previousState;
        private int _repeatedButtonTimer;
        private float _rightCurVibrationStrength;
        private bool _rightStickDown;
        private bool _rightTriggerDown;
        private bool _rightVibrating;
        private TimeSpan _vibrationDurationLeft;
        private TimeSpan _vibrationDurationRight;
        private TimeSpan _vibrationStart;

        private float _vibrationStrengthLeft;
        private float _vibrationStrengthRight;

        public GamePadListener()
            : this(new GamePadListenerSettings())
        {
        }

        public GamePadListener(GamePadListenerSettings settings)
        {
            PlayerIndex = settings.PlayerIndex;
            VibrationEnabled = settings.VibrationEnabled;
            VibrationStrengthLeft = settings.VibrationStrengthLeft;
            VibrationStrengthRight = settings.VibrationStrengthRight;
            ThumbStickDeltaTreshold = settings.ThumbStickDeltaTreshold;
            ThumbstickDownTreshold = settings.ThumbstickDownTreshold;
            TriggerDeltaTreshold = settings.TriggerDeltaTreshold;
            TriggerDownTreshold = settings.TriggerDownTreshold;
            RepeatInitialDelay = settings.RepeatInitialDelay;
            RepeatDelay = settings.RepeatDelay;

            _previousGameTime = new GameTime();
            _previousState = new GamePadState();
        }

        /// <summary>
        ///     If set to true, the static event <see cref="ControllerConnectionChanged" />
        ///     will fire when any controller changes in connectivity status.
        ///     <para>
        ///         This functionality requires that you have one actively updating
        ///         <see cref="InputListenerManager" />.
        ///     </para>
        /// </summary>
        public static bool CheckControllerConnections { get; set; }

        /// <summary>
        ///     The index of the controller.
        /// </summary>
        public PlayerIndex PlayerIndex { get; }

        /// <summary>
        ///     When a button is held down, the interval in which
        ///     ButtonRepeated fires. Value in milliseconds.
        /// </summary>
        public int RepeatDelay { get; }

        /// <summary>
        ///     The amount of time a button has to be held down
        ///     in order to fire ButtonRepeated the first time.
        ///     Value in milliseconds.
        /// </summary>
        public int RepeatInitialDelay { get; }

        /// <summary>
        ///     Whether vibration is enabled for this controller.
        /// </summary>
        public bool VibrationEnabled { get; set; }

        /// <summary>
        ///     General setting for the strength of the left motor.
        ///     This motor has a slow, deep, powerful rumble.
        ///     <para>
        ///         This setting will modify all future vibrations
        ///         through this listener.
        ///     </para>
        /// </summary>
        public float VibrationStrengthLeft
        {
            get { return _vibrationStrengthLeft; }
            // Clamp the value, just to be sure.
            set { _vibrationStrengthLeft = MathHelper.Clamp(value, 0, 1); }
        }

        /// <summary>
        ///     General setting for the strength of the right motor.
        ///     This motor has a snappy, quick, high-pitched rumble.
        ///     <para>
        ///         This setting will modify all future vibrations
        ///         through this listener.
        ///     </para>
        /// </summary>
        public float VibrationStrengthRight
        {
            get { return _vibrationStrengthRight; }
            // Clamp the value, just to be sure.
            set { _vibrationStrengthRight = MathHelper.Clamp(value, 0, 1); }
        }

        /// <summary>
        ///     The treshold of movement that has to be met in order
        ///     for the listener to fire an event with the trigger's
        ///     updated position.
        ///     <para>
        ///         In essence this defines the event's
        ///         resolution.
        ///     </para>
        ///     At a value of 0 this will fire every time
        ///     the trigger's position is not 0f.
        /// </summary>
        public float TriggerDeltaTreshold { get; }

        /// <summary>
        ///     The treshold of movement that has to be met in order
        ///     for the listener to fire an event with the thumbstick's
        ///     updated position.
        ///     <para>
        ///         In essence this defines the event's
        ///         resolution.
        ///     </para>
        ///     At a value of 0 this will fire every time
        ///     the thumbstick's position is not {x:0, y:0}.
        /// </summary>
        public float ThumbStickDeltaTreshold { get; }

        /// <summary>
        ///     How deep the triggers have to be depressed in order to
        ///     register as a ButtonDown event.
        /// </summary>
        public float TriggerDownTreshold { get; }

        /// <summary>
        ///     How deep the triggers have to be depressed in order to
        ///     register as a ButtonDown event.
        /// </summary>
        public float ThumbstickDownTreshold { get; }

        /// <summary>
        ///     This event fires whenever a controller connects or disconnects.
        ///     <para>
        ///         In order
        ///         for it to work, the <see cref="CheckControllerConnections" /> property must
        ///         be set to true.
        ///     </para>
        /// </summary>
        public static event EventHandler<GamePadEventArgs> ControllerConnectionChanged;

        /// <summary>
        ///     This event fires whenever a button changes from the Up
        ///     to the Down state.
        /// </summary>
        public event EventHandler<GamePadEventArgs> ButtonDown;

        /// <summary>
        ///     This event fires whenever a button changes from the Down
        ///     to the Up state.
        /// </summary>
        public event EventHandler<GamePadEventArgs> ButtonUp;

        /// <summary>
        ///     This event fires repeatedly whenever a button is held sufficiently
        ///     long. Use this for things like menu navigation.
        /// </summary>
        public event EventHandler<GamePadEventArgs> ButtonRepeated;

        /// <summary>
        ///     This event fires whenever a thumbstick changes position.
        ///     <para>
        ///         The parameter governing the sensitivity of this functionality
        ///         is <see cref="GamePadListenerSettings.ThumbStickDeltaTreshold" />.
        ///     </para>
        /// </summary>
        public event EventHandler<GamePadEventArgs> ThumbStickMoved;

        /// <summary>
        ///     This event fires whenever a trigger changes position.
        ///     <para>
        ///         The parameter governing the sensitivity of this functionality
        ///         is <see cref="GamePadListenerSettings.TriggerDeltaTreshold" />.
        ///     </para>
        /// </summary>
        public event EventHandler<GamePadEventArgs> TriggerMoved;


        /// <summary>
        ///     Send a vibration command to the controller.
        ///     Returns true if the operation succeeded.
        ///     <para>
        ///         Motor values that are unset preserve
        ///         their current vibration strength and duration.
        ///     </para>
        ///     Note: Vibration currently only works on select platforms,
        ///     like Monogame.Windows.
        /// </summary>
        /// <param name="durationMs">Duration of the vibration in milliseconds.</param>
        /// <param name="leftStrength">
        ///     The strength of the left motor.
        ///     This motor has a slow, deep, powerful rumble.
        /// </param>
        /// <param name="rightStrength">
        ///     The strength of the right motor.
        ///     This motor has a snappy, quick, high-pitched rumble.
        /// </param>
        /// <returns>Returns true if the operation succeeded.</returns>
        public bool Vibrate(int durationMs, float leftStrength = float.NegativeInfinity,
            float rightStrength = float.NegativeInfinity)
        {
            if (!VibrationEnabled)
                return false;

            var lstrength = MathHelper.Clamp(leftStrength, 0, 1);
            var rstrength = MathHelper.Clamp(rightStrength, 0, 1);

            if (float.IsNegativeInfinity(leftStrength))
                lstrength = _leftCurVibrationStrength;
            if (float.IsNegativeInfinity(rightStrength))
                rstrength = _rightCurVibrationStrength;

            var success = GamePad.SetVibration(PlayerIndex, lstrength*VibrationStrengthLeft,
                rstrength*VibrationStrengthRight);
            if (success)
            {
                _leftVibrating = true;
                _rightVibrating = true;

                if (leftStrength > 0)
                    _vibrationDurationLeft = new TimeSpan(0, 0, 0, 0, durationMs);
                else
                {
                    if (lstrength > 0)
                        _vibrationDurationLeft -= _gameTime.TotalGameTime - _vibrationStart;
                    else
                        _leftVibrating = false;
                }

                if (rightStrength > 0)
                    _vibrationDurationRight = new TimeSpan(0, 0, 0, 0, durationMs);
                else
                {
                    if (rstrength > 0)
                        _vibrationDurationRight -= _gameTime.TotalGameTime - _vibrationStart;
                    else
                        _rightVibrating = false;
                }

                _vibrationStart = _gameTime.TotalGameTime;

                _leftCurVibrationStrength = lstrength;
                _rightCurVibrationStrength = rstrength;
            }
            return success;
        }

        private void CheckAllButtons()
        {
            // PacketNumber only and always changes if there is a difference between GamePadStates.
            // ...At least, that's the theory. It doesn't seem to be implemented. Disabled for now.
            //if (_lastPacketNumber == _currentState.PacketNumber)
            //    return;
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (_excludedButtons.Contains(button))
                    break;
                if (_currentState.IsButtonDown(button) && _previousState.IsButtonUp(button))
                    RaiseButtonDown(button);
                if (_currentState.IsButtonUp(button) && _previousState.IsButtonDown(button))
                    RaiseButtonUp(button);
            }

            // Checks triggers as buttons and floats
            CheckTriggers(s => s.Triggers.Left, Buttons.LeftTrigger);
            CheckTriggers(s => s.Triggers.Right, Buttons.RightTrigger);

            // Checks thumbsticks as vector2s
            CheckThumbSticks(s => s.ThumbSticks.Right, Buttons.RightStick);
            CheckThumbSticks(s => s.ThumbSticks.Left, Buttons.LeftStick);
        }

        private void CheckTriggers(Func<GamePadState, float> getButtonState, Buttons button)
        {
            var debounce = 0.05f; // Value used to qualify a trigger as coming Up from a Down state
            var curstate = getButtonState(_currentState);
            var curdown = curstate > TriggerDownTreshold;
            var prevdown = button == Buttons.RightTrigger ? _rightTriggerDown : _leftTriggerDown;

            if (!prevdown && curdown)
            {
                RaiseButtonDown(button);
                if (button == Buttons.RightTrigger)
                    _rightTriggerDown = true;
                else
                    _leftTriggerDown = true;
            }
            else
            {
                if (prevdown && (curstate < debounce))
                {
                    RaiseButtonUp(button);
                    if (button == Buttons.RightTrigger)
                        _rightTriggerDown = false;
                    else
                        _leftTriggerDown = false;
                }
            }

            var prevstate = getButtonState(_lastTriggerState);
            if (curstate > TriggerDeltaTreshold)
            {
                if (Math.Abs(prevstate - curstate) >= TriggerDeltaTreshold)
                {
                    TriggerMoved?.Invoke(this, MakeArgs(button, curstate));
                    _lastTriggerState = _currentState;
                }
            }
            else
            {
                if (prevstate > TriggerDeltaTreshold)
                {
                    TriggerMoved?.Invoke(this, MakeArgs(button, curstate));
                    _lastTriggerState = _currentState;
                }
            }
        }

        private void CheckThumbSticks(Func<GamePadState, Vector2> getButtonState, Buttons button)
        {
            const float debounce = 0.15f;
            var curVector = getButtonState(_currentState);
            var curdown = curVector.Length() > ThumbstickDownTreshold;
            var right = button == Buttons.RightStick;
            var prevdown = right ? _rightStickDown : _leftStickDown;

            var prevdir = button == Buttons.RightStick ? _lastRightStickDirection : _lastLeftStickDirection;
            Buttons curdir;
            if (curVector.Y > curVector.X)
            {
                if (curVector.Y > -curVector.X)
                    curdir = right ? Buttons.RightThumbstickUp : Buttons.LeftThumbstickUp;
                else
                    curdir = right ? Buttons.RightThumbstickLeft : Buttons.LeftThumbstickLeft;
            }
            else
            {
                if (curVector.Y < -curVector.X)
                    curdir = right ? Buttons.RightThumbstickDown : Buttons.LeftThumbstickDown;
                else
                    curdir = right ? Buttons.RightThumbstickRight : Buttons.LeftThumbstickRight;
            }

            if (!prevdown && curdown)
            {
                if (right)
                    _lastRightStickDirection = curdir;
                else
                    _lastLeftStickDirection = curdir;

                RaiseButtonDown(curdir);
                if (button == Buttons.RightStick)
                    _rightStickDown = true;
                else
                    _leftStickDown = true;
            }
            else
            {
                if (prevdown && (curVector.Length() < debounce))
                {
                    RaiseButtonUp(prevdir);
                    if (button == Buttons.RightStick)
                        _rightStickDown = false;
                    else
                        _leftStickDown = false;
                }
                else
                {
                    if (prevdown && curdown && (curdir != prevdir))
                    {
                        RaiseButtonUp(prevdir);
                        if (right)
                            _lastRightStickDirection = curdir;
                        else
                            _lastLeftStickDirection = curdir;
                        RaiseButtonDown(curdir);
                    }
                }
            }

            var prevVector = getButtonState(_lastThumbStickState);
            if (curVector.Length() > ThumbStickDeltaTreshold)
            {
                if (Vector2.Distance(curVector, prevVector) >= ThumbStickDeltaTreshold)
                {
                    ThumbStickMoved?.Invoke(this, MakeArgs(button, thumbStickState: curVector));
                    _lastThumbStickState = _currentState;
                }
            }
            else
            {
                if (prevVector.Length() > ThumbStickDeltaTreshold)
                {
                    ThumbStickMoved?.Invoke(this, MakeArgs(button, thumbStickState: curVector));
                    _lastThumbStickState = _currentState;
                }
            }
        }

        internal static void CheckConnections()
        {
            if (!CheckControllerConnections)
                return;

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                if (GamePad.GetState(index).IsConnected ^ _gamePadConnections[(int) index])
                    // We need more XORs in this world
                {
                    _gamePadConnections[(int) index] = !_gamePadConnections[(int) index];
                    ControllerConnectionChanged?.Invoke(null,
                        new GamePadEventArgs(new GamePadState(), GamePad.GetState(index), TimeSpan.Zero, index));
                }
            }
        }

        private void CheckVibrate()
        {
            if (_leftVibrating && (_vibrationStart + _vibrationDurationLeft < _gameTime.TotalGameTime))
                Vibrate(0, 0);
            if (_rightVibrating && (_vibrationStart + _vibrationDurationRight < _gameTime.TotalGameTime))
                Vibrate(0, rightStrength: 0);
        }

        public override void Update(GameTime gameTime)
        {
            _gameTime = gameTime;
            _currentState = GamePad.GetState(PlayerIndex);
            CheckVibrate();
            if (!_currentState.IsConnected)
                return;
            CheckAllButtons();
            CheckRepeatButton();
            //_lastPacketNumber = _currentState.PacketNumber;
            _previousGameTime = gameTime;
            _previousState = _currentState;
        }

        private GamePadEventArgs MakeArgs(Buttons? button,
            float triggerstate = 0, Vector2? thumbStickState = null)
        {
            var elapsedTime = _gameTime.TotalGameTime - _previousGameTime.TotalGameTime;
            return new GamePadEventArgs(_previousState, _currentState,
                elapsedTime, PlayerIndex, button, triggerstate, thumbStickState);
        }

        private void RaiseButtonDown(Buttons button)
        {
            ButtonDown?.Invoke(this, MakeArgs(button));
            ButtonRepeated?.Invoke(this, MakeArgs(button));
            _lastButton = button;
            _repeatedButtonTimer = 0;
        }

        private void RaiseButtonUp(Buttons button)
        {
            ButtonUp?.Invoke(this, MakeArgs(button));
            _lastButton = 0;
        }

        private void CheckRepeatButton()
        {
            _repeatedButtonTimer += _gameTime.ElapsedGameTime.Milliseconds;

            if ((_repeatedButtonTimer < RepeatInitialDelay) || (_lastButton == 0))
                return;

            if (_repeatedButtonTimer < RepeatInitialDelay + RepeatDelay)
            {
                ButtonRepeated?.Invoke(this, MakeArgs(_lastButton));
                _repeatedButtonTimer = RepeatDelay + RepeatInitialDelay;
            }
            else
            {
                if (_repeatedButtonTimer > RepeatInitialDelay + RepeatDelay*2)
                {
                    ButtonRepeated?.Invoke(this, MakeArgs(_lastButton));
                    _repeatedButtonTimer = RepeatDelay + RepeatInitialDelay;
                }
            }
        }
    }
}