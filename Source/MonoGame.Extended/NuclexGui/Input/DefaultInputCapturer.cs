using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.InputListeners.Devices;

namespace MonoGame.Extended.NuclexGui.Input
{
    public class DefaultInputCapturer : IInputCapturer, IDisposable
    {
        #region class DummyInputReceiver

        /// <summary>Dummy receiver for input events</summary>
        private class DummyInputReceiver : IInputReceiver
        {

            /// <summary>Default instance of the dummy receiver</summary>
            public static readonly DummyInputReceiver Default = new DummyInputReceiver();

            /// <summary>Injects an input command into the input receiver</summary>
            /// <param name="command">Input command to be injected</param>
            public void InjectCommand(Command command) { }

            /// <summary>Called when a button on the gamepad has been pressed</summary>
            /// <param name="button">Button that has been pressed</param>
            public void InjectButtonPress(Buttons button) { }

            /// <summary>Called when a button on the gamepad has been released</summary>
            /// <param name="button">Button that has been released</param>
            public void InjectButtonRelease(Buttons button) { }

            /// <summary>Injects a mouse position update into the receiver</summary>
            /// <param name="x">New X coordinate of the mouse cursor on the screen</param>
            /// <param name="y">New Y coordinate of the mouse cursor on the screen</param>
            public void InjectMouseMove(float x, float y) { }

            /// <summary>Called when a mouse button has been pressed down</summary>
            /// <param name="button">Index of the button that has been pressed</param>
            public void InjectMousePress(MouseButton button) { }

            /// <summary>Called when a mouse button has been released again</summary>
            /// <param name="button">Index of the button that has been released</param>
            public void InjectMouseRelease(MouseButton button) { }

            /// <summary>Called when the mouse wheel has been rotated</summary>
            /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
            public void InjectMouseWheel(float ticks) { }

            /// <summary>Called when a key on the keyboard has been pressed down</summary>
            /// <param name="keyCode">Code of the key that was pressed</param>
            public void InjectKeyPress(Keys keyCode) { }

            /// <summary>Called when a key on the keyboard has been released again</summary>
            /// <param name="keyCode">Code of the key that was released</param>
            public void InjectKeyRelease(Keys keyCode) { }

            /// <summary>Handle user text input by a physical or virtual keyboard</summary>
            /// <param name="character">Character that has been entered</param>
            public void InjectCharacter(char character) { }

        }

        #endregion // class DummyInputReceiver

        #region Properties

        /// <summary>Input receiver any captured input will be sent to</summary>
        public IInputReceiver InputReceiver
        {
            get
            {
                if (ReferenceEquals(_inputReceiver, DummyInputReceiver.Default))
                    return null;
                else
                    return _inputReceiver;
            }
            set
            {
                if (value == null)
                    _inputReceiver = DummyInputReceiver.Default;
                else
                    _inputReceiver = value;
            }
        }

        #endregion

        #region Fields

        /// <summary>Player index this input capturer is working with</summary>
        private ExtendedPlayerIndex _playerIndex;

        /// <summary>Current receiver of input events</summary>
        /// <remarks>
        ///   Always valid. If no input receiver is assigned, this field will be set
        ///   to a dummy receiver.
        /// </remarks>
        private IInputReceiver _inputReceiver;

        /// <summary>Input service the capturer is currently subscribed to</summary>
        private InputListeners.IInputService _inputService;

        /// <summary>Keyboard the input capturer is subscribed to</summary>
        private IKeyboard _subscribedKeyboard;
        /// <summary>Mouse the input capturer is subscribed to</summary>
        private IMouse _subscribedMouse;
        /// <summary>Game pad the input capturer is subscribed to</summary>
        private IGamePad _subscribedGamePad;
        /// <summary>Chat pad the input capturer is subscribed to</summary>
        private IKeyboard _subscribedChatPad;

        /// <summary>Delegate for the keyPressed() method</summary>
        private KeyDelegate _keyPressedDelegate;
        /// <summary>Delegate for the keyReleased() method</summary>
        private KeyDelegate _keyReleasedDelegate;
        /// <summary>Delegate for the characterEntered() method</summary>
        private CharacterDelegate _characterEnteredDelegate;
        /// <summary>Delegate for the mouseButtonPressed() method</summary>
        private MouseButtonDelegate _mouseButtonPressedDelegate;
        /// <summary>Delegate for the mouseButtonReleased() method</summary>
        private MouseButtonDelegate _mouseButtonReleasedDelegate;
        /// <summary>Delegate for the mouseMoved() method</summary>
        private MouseMoveDelegate _mouseMovedDelegate;
        /// <summary>Delegate for the mouseWheelRotated() method</summary>
        private MouseWheelDelegate _mouseWheelRotatedDelegate;
        /// <summary>Delegate for the buttonPressed() method</summary>
        private GamePadButtonDelegate _buttonPressedDelegate;
        /// <summary>Delegate for the buttonReleased() method</summary>
        private GamePadButtonDelegate _buttonReleasedDelegate;

        #endregion

        #region Constructors

        /// <summary>Initializes a new input capturer, taking the input service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the input capturer will take the input service from</param>
        public DefaultInputCapturer(IServiceProvider serviceProvider) : this(getInputService(serviceProvider))
        { }

        /// <summary>Initializes a new input capturer using the specified input service</summary>
        /// <param name="inputService">Input service the capturer will subscribe to</param>
        public DefaultInputCapturer(InputListeners.IInputService inputService)
        {
            _inputService = inputService;
            _inputReceiver = new DummyInputReceiver();
            _playerIndex = ExtendedPlayerIndex.One;

            _keyPressedDelegate = new KeyDelegate(keyPressed);
            _keyReleasedDelegate = new KeyDelegate(keyReleased);
            _characterEnteredDelegate = new CharacterDelegate(characterEntered);
            _mouseButtonPressedDelegate = new MouseButtonDelegate(mouseButtonPressed);
            _mouseButtonReleasedDelegate = new MouseButtonDelegate(mouseButtonReleased);
            _mouseMovedDelegate = new MouseMoveDelegate(mouseMoved);
            _mouseWheelRotatedDelegate = new MouseWheelDelegate(mouseWheelRotated);
            _buttonPressedDelegate = new GamePadButtonDelegate(buttonPressed);
            _buttonReleasedDelegate = new GamePadButtonDelegate(buttonReleased);

            subscribeInputDevices();
        }

        #endregion

        #region Methods

        /// <summary>Immediately releases all resources owned by the instance</summary>
        public void Dispose()
        {
            if (_inputService != null)
            {
                unsubscribeInputDevices();
                _inputService = null;
            }
        }

        /// <summary>Changes the controller which can interact with the GUI</summary>
        /// <param name="playerIndex">
        ///   Index of the player whose controller will be allowed to interact with the GUI
        /// </param>
        public void ChangePlayerIndex(PlayerIndex playerIndex)
        {
            ChangePlayerIndex((ExtendedPlayerIndex)playerIndex);
        }

        /// <summary>Changes the controller which can interact with the GUI</summary>
        /// <param name="playerIndex">
        ///   Index of the player whose controller will be allowed to interact with the GUI
        /// </param>
        public void ChangePlayerIndex(ExtendedPlayerIndex playerIndex)
        {
            unsubscribePlayerSpecificInputDevices();
            _playerIndex = playerIndex;
            subscribePlayerSpecificDevices();
        }

        /// <summary>Subscribes to the events of all input devices</summary>
        private void subscribeInputDevices()
        {
            _subscribedKeyboard = _inputService.GetKeyboard();
            _subscribedKeyboard.KeyPressed += _keyPressedDelegate;
            _subscribedKeyboard.KeyReleased += _keyReleasedDelegate;
            _subscribedKeyboard.CharacterEntered += _characterEnteredDelegate;

            _subscribedMouse = _inputService.GetMouse();
            _subscribedMouse.MouseButtonPressed += _mouseButtonPressedDelegate;
            _subscribedMouse.MouseButtonReleased += _mouseButtonReleasedDelegate;
            _subscribedMouse.MouseMoved += _mouseMovedDelegate;
            _subscribedMouse.MouseWheelRotated += _mouseWheelRotatedDelegate;

            subscribePlayerSpecificDevices();
        }

        /// <summary>Subscribes to the events of all player-specific input devices</summary>
        private void subscribePlayerSpecificDevices()
        {
            _subscribedGamePad = _inputService.GetGamePad(_playerIndex);
            _subscribedGamePad.ButtonPressed += _buttonPressedDelegate;
            _subscribedGamePad.ButtonReleased += _buttonReleasedDelegate;

            if (_playerIndex < ExtendedPlayerIndex.Five)
            {
                var standardPlayerIndex = (PlayerIndex)_playerIndex;
                _subscribedChatPad = _inputService.GetKeyboard(standardPlayerIndex);
                _subscribedChatPad.KeyPressed += _keyPressedDelegate;
                _subscribedChatPad.KeyReleased += _keyReleasedDelegate;
                _subscribedChatPad.CharacterEntered += _characterEnteredDelegate;
            }
        }

        /// <summary>Unsubscribes from the events of all input devices</summary>
        private void unsubscribeInputDevices()
        {
            unsubscribePlayerSpecificInputDevices();

            if (_subscribedKeyboard != null)
            {
                _subscribedKeyboard.CharacterEntered -= _characterEnteredDelegate;
                _subscribedKeyboard.KeyReleased -= _keyReleasedDelegate;
                _subscribedKeyboard.KeyPressed -= _keyPressedDelegate;
                _subscribedKeyboard = null;
            }
            if (_subscribedMouse != null)
            {
                _subscribedMouse.MouseWheelRotated -= _mouseWheelRotatedDelegate;
                _subscribedMouse.MouseMoved -= _mouseMovedDelegate;
                _subscribedMouse.MouseButtonReleased -= _mouseButtonReleasedDelegate;
                _subscribedMouse.MouseButtonPressed -= _mouseButtonPressedDelegate;
                _subscribedMouse = null;
            }
        }

        /// <summary>Unsubscribes from the events of all player-specific input devices</summary>
        private void unsubscribePlayerSpecificInputDevices()
        {
            if (_subscribedChatPad != null)
            {
                _subscribedChatPad.CharacterEntered -= _characterEnteredDelegate;
                _subscribedChatPad.KeyReleased -= _keyReleasedDelegate;
                _subscribedChatPad.KeyPressed -= _keyPressedDelegate;
                _subscribedChatPad = null;
            }

            if (_subscribedGamePad != null)
            {
                _subscribedGamePad.ButtonPressed -= _buttonPressedDelegate;
                _subscribedGamePad.ButtonReleased -= _buttonReleasedDelegate;
                _subscribedGamePad = null;
            }
        }

        /// <summary>Called when a button on the game pad has been released</summary>
        /// <param name="buttons">Button that has been released</param>
        private void buttonReleased(Buttons buttons)
        {
            _inputReceiver.InjectButtonRelease(buttons);
        }

        /// <summary>Called when a button on the game pad has been pressed</summary>
        /// <param name="buttons">Button that has been pressed</param>
        private void buttonPressed(Buttons buttons)
        {
            if ((buttons & Buttons.DPadUp) != 0)
                _inputReceiver.InjectCommand(Command.Up);
            else if ((buttons & Buttons.DPadDown) != 0)
                _inputReceiver.InjectCommand(Command.Down);
            else if ((buttons & Buttons.DPadLeft) != 0)
                _inputReceiver.InjectCommand(Command.Left);
            else if ((buttons & Buttons.DPadRight) != 0)
                _inputReceiver.InjectCommand(Command.Right);
            else
                _inputReceiver.InjectButtonPress(buttons);
        }

        /// <summary>Called when the mouse wheel has been rotated</summary>
        /// <param name="ticks">Number of ticks the wheel was rotated</param>
        private void mouseWheelRotated(float ticks)
        {
            _inputReceiver.InjectMouseWheel(ticks);
        }

        /// <summary>Called when the mouse cursor has been moved</summary>
        /// <param name="x">New X coordinate of the mouse cursor</param>
        /// <param name="y">New Y coordinate of the mouse cursor</param>
        private void mouseMoved(float x, float y)
        {
            _inputReceiver.InjectMouseMove(x, y);
        }

        /// <summary>Called when a mouse button has been released</summary>
        /// <param name="buttons">Mouse button that has been released</param>
        private void mouseButtonReleased(MouseButton buttons)
        {
            _inputReceiver.InjectMouseRelease(buttons);
        }

        /// <summary>Called when a mouse button has been pressed</summary>
        /// <param name="buttons">Mouse button that has been pressed</param>
        private void mouseButtonPressed(MouseButton buttons)
        {
            _inputReceiver.InjectMousePress(buttons);
        }

        /// <summary>Called when a character has been entered on the keyboard</summary>
        /// <param name="character">Character that has been entered</param>
        private void characterEntered(char character)
        {
            _inputReceiver.InjectCharacter(character);
        }

        /// <summary>Called when a key has been released</summary>
        /// <param name="key">Key that was released</param>
        private void keyReleased(Keys key)
        {
            _inputReceiver.InjectKeyRelease(key);
        }

        /// <summary>Called when a key has been pressed</summary>
        /// <param name="key">Key that was pressed</param>
        private void keyPressed(Keys key)
        {
            _inputReceiver.InjectKeyPress(key);
        }

        /// <summary>Retrieves the input service from a service provider</summary>
        /// <param name="serviceProvider">
        ///   Service provider the service is taken from
        /// </param>
        /// <returns>The input service stored in the service provider</returns>
        private static InputListeners.IInputService getInputService(IServiceProvider serviceProvider)
        {
            var inputService = (InputListeners.IInputService)serviceProvider.GetService(typeof(InputListeners.IInputService));

            if (inputService == null)
                throw new InvalidOperationException(
                  "Using the GUI with the DefaultInputCapturer requires the IInputService. " +
                  "Please either add the IInputService to Game.Services by using the " +
                  "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
                  "implementation for the GUI and assign it before GuiManager.Initialize() " +
                  "is called."
                );

            return inputService;
        }

        #endregion
    }
}
