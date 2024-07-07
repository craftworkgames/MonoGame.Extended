using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Input.InputListeners
{
    /// <summary>
    ///     This is a class that contains settings to be used to initialise a <see cref="GamePadListener" />.
    /// </summary>
    /// <seealso cref="InputListenerManager" />
    public class GamePadListenerSettings : InputListenerSettings<GamePadListener>
    {
        public GamePadListenerSettings()
            : this(PlayerIndex.One)
        {
        }

        /// <summary>
        ///     This is a class that contains settings to be used to initialise a <see cref="GamePadListener" />.
        ///     <para>Note: There are a number of extra settings that are settable properties.</para>
        /// </summary>
        /// <param name="playerIndex">The index of the controller the listener will be tied to.</param>
        /// <param name="vibrationEnabled">Whether vibration is enabled on the controller.</param>
        /// <param name="vibrationStrengthLeft">
        ///     General setting for the strength of the left motor.
        ///     This motor has a slow, deep, powerful rumble.
        ///     This setting will modify all future vibrations
        ///     through this listener.
        /// </param>
        /// <param name="vibrationStrengthRight">
        ///     General setting for the strength of the right motor.
        ///     This motor has a snappy, quick, high-pitched rumble.
        ///     This setting will modify all future vibrations
        ///     through this listener.
        /// </param>
        public GamePadListenerSettings(PlayerIndex playerIndex, bool vibrationEnabled = true,
            float vibrationStrengthLeft = 1.0f, float vibrationStrengthRight = 1.0f)
        {
            PlayerIndex = playerIndex;
            VibrationEnabled = vibrationEnabled;
            VibrationStrengthLeft = vibrationStrengthLeft;
            VibrationStrengthRight = vibrationStrengthRight;
            TriggerDownTreshold = 0.15f;
            ThumbstickDownTreshold = 0.5f;
            RepeatInitialDelay = 500;
            RepeatDelay = 50;
        }

        /// <summary>
        ///     The index of the controller.
        /// </summary>
        public PlayerIndex PlayerIndex { get; set; }

        /// <summary>
        ///     When a button is held down, the interval in which
        ///     ButtonRepeated fires. Value in milliseconds.
        /// </summary>
        public int RepeatDelay { get; set; }

        /// <summary>
        ///     The amount of time a button has to be held down
        ///     in order to fire ButtonRepeated the first time.
        ///     Value in milliseconds.
        /// </summary>
        public int RepeatInitialDelay { get; set; }


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
        public float VibrationStrengthLeft { get; set; }

        /// <summary>
        ///     General setting for the strength of the right motor.
        ///     This motor has a snappy, quick, high-pitched rumble.
        ///     <para>
        ///         This setting will modify all future vibrations
        ///         through this listener.
        ///     </para>
        /// </summary>
        public float VibrationStrengthRight { get; set; }

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
        public float TriggerDeltaTreshold { get; set; }

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
        public float ThumbStickDeltaTreshold { get; set; }

        /// <summary>
        ///     How deep the triggers have to be depressed in order to
        ///     register as a ButtonDown event.
        /// </summary>
        public float TriggerDownTreshold { get; set; }

        /// <summary>
        ///     How deep the triggers have to be depressed in order to
        ///     register as a ButtonDown event.
        /// </summary>
        public float ThumbstickDownTreshold { get; private set; }

        public override GamePadListener CreateListener()
        {
            return new GamePadListener(this);
        }
    }
}