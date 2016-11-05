using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners.Devices;

namespace MonoGame.Extended.InputListeners
{
    /// <summary>Provides access to the game's input devices</summary>
    /// <remarks>
    ///   This interface provides a uniform way to access all input devices available
    ///   to the system. It also allows XNA games to interface with standard game pads
    ///   and joysticks through DirectInput.
    /// </remarks>
    public interface IInputService
    {
        /// <summary>All keyboards known to the system</summary>
        ReadOnlyCollection<IKeyboard> Keyboards { get; }

        /// <summary>All mice known to the system</summary>
        ReadOnlyCollection<IMouse> Mice { get; }

        /// <summary>All game pads known to the system</summary>
        ReadOnlyCollection<IGamePad> GamePads { get; }

        /// <summary>All touch panels known to the system</summary>
        ReadOnlyCollection<ITouchPanel> TouchPanels { get; }

        /// <summary>Returns the primary mouse input device</summary>
        /// <returns>The primary mouse</returns>
        IMouse GetMouse();

        /// <summary>Returns the keyboard on a PC</summary>
        /// <returns>The keyboard</returns>
        IKeyboard GetKeyboard();

        /// <summary>Returns the chat pad for the specified player</summary>
        /// <param name="playerIndex">Player whose chat pad will be returned</param>
        /// <returns>The chat pad of the specified player</returns>
        IKeyboard GetKeyboard(PlayerIndex playerIndex);

        /// <summary>Returns the game pad for the specified player</summary>
        /// <param name="playerIndex">Player whose game pad will be returned</param>
        /// <returns>The game pad of the specified player</returns>
        /// <remarks>
        ///   This will only return the XINPUT devices (aka XBox 360 controllers)
        ///   attached. Any standard game pads attached to a PC can only be
        ///   returned through the ExtendedPlayerIndex overload where they will
        ///   take the places of game pads for player 5 and upwards.
        /// </remarks>
        IGamePad GetGamePad(PlayerIndex playerIndex);

        /// <summary>Returns the game pad for the specified player</summary>
        /// <param name="playerIndex">Player whose game pad will be returned</param>
        /// <returns>The game pad of the specified player</returns>
        IGamePad GetGamePad(ExtendedPlayerIndex playerIndex);

        /// <summary>Returns the touch panel on the system</summary>
        /// <returns>The system's touch panel</returns>
        ITouchPanel GetTouchPanel();

        /// <summary>Updates the state of all input devices</summary>
        /// <remarks>
        ///   <para>
        ///     If this method is called with no snapshots in the queue, it will
        ///     query the state of all input devices immediately, raising events
        ///     for any changed states. This way, you can ignore the entire
        ///     snapshot system if you just want basic input device access.
        ///   </para>
        ///   <para>
        ///     If this method is called while one or more snapshots are waiting in
        ///     the queue, this method takes the next snapshot from the queue and makes
        ///     it the current state of all active devices.
        ///   </para>
        /// </remarks>
        void Update();

        /// <summary>Takes a snapshot of the current state of all input devices</summary>
        /// <remarks>
        ///   This snapshot will be queued until the user calls the Update() method,
        ///   where the next polled snapshot will be taken from the queue and provided
        ///   to the user.
        /// </remarks>
        void TakeSnapshot();

        /// <summary>Number of snapshots currently in the queue</summary>
        int SnapshotCount { get; }
    }
}
