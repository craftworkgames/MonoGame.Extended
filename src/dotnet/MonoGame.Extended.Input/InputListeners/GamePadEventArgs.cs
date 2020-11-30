using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input.InputListeners
{
    /// <summary>
    ///     This class contains all information resulting from events fired by
    ///     <see cref="GamePadListener" />.
    /// </summary>
    public class GamePadEventArgs : EventArgs
    {
        public GamePadEventArgs(GamePadState previousState, GamePadState currentState,
            TimeSpan elapsedTime, PlayerIndex playerIndex, Buttons? button = null,
            float triggerState = 0, Vector2? thumbStickState = null)
        {
            PlayerIndex = playerIndex;
            PreviousState = previousState;
            CurrentState = currentState;
            ElapsedTime = elapsedTime;
            if (button != null)
                Button = button.Value;
            TriggerState = triggerState;
            ThumbStickState = thumbStickState ?? Vector2.Zero;
        }

        /// <summary>
        ///     The index of the controller.
        /// </summary>
        public PlayerIndex PlayerIndex { get; private set; }

        /// <summary>
        ///     The state of the controller in the previous update.
        /// </summary>
        public GamePadState PreviousState { get; private set; }

        /// <summary>
        ///     The state of the controller in this update.
        /// </summary>
        public GamePadState CurrentState { get; private set; }

        /// <summary>
        ///     The button that triggered this event, if appliable.
        /// </summary>
        public Buttons Button { get; private set; }

        /// <summary>
        ///     The time elapsed since last event.
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        ///     If a TriggerMoved event, displays the responsible trigger's position.
        /// </summary>
        public float TriggerState { get; private set; }

        /// <summary>
        ///     If a ThumbStickMoved event, displays the responsible stick's position.
        /// </summary>
        public Vector2 ThumbStickState { get; private set; }
    }
}