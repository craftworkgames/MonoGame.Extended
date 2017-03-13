using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Input
{
    public class DefaultInputCapturer : IInputCapturer, IDisposable
    {
        private bool _disposedValue; // To detect redundant calls
        private readonly GamePadListener _gamePadListener;

        /// <summary>Current receiver of input events</summary>
        /// <remarks>
        ///     Always valid. If no input receiver is assigned, this field will be set
        ///     to a dummy receiver.
        /// </remarks>
        private IInputReceiver _inputReceiver;

        /// <summary>Input service the capturer is currently subscribed to</summary>
        private IGuiInputService _inputService;

        private readonly KeyboardListener _keyboardListener;
        private readonly MouseListener _mouseListener;

        /// <summary>Initializes a new input capturer, taking the input service from a service provider</summary>
        /// <param name="serviceProvider">Service provider the input capturer will take the input service from</param>
        public DefaultInputCapturer(IServiceProvider serviceProvider) : this(GetInputService(serviceProvider))
        {
        }

        /// <summary>Initializes a new input capturer using the specified input service</summary>
        /// <param name="inputService">Input service the capturer will subscribe to</param>
        public DefaultInputCapturer(IGuiInputService inputService)
        {
            _inputService = inputService;
            _inputReceiver = new DummyInputReceiver();

            _keyboardListener = inputService.KeyboardListener;
            _mouseListener = inputService.MouseListener;
            _gamePadListener = inputService.GamePadListener;

            SubscribeInputDevices();
        }

        // ~MainInputCapturer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        /// <summary>Input receiver any captured input will be sent to</summary>
        public IInputReceiver InputReceiver
        {
            get
            {
                if (ReferenceEquals(_inputReceiver, DummyInputReceiver.Default))
                    return null;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_inputService != null)
                    {
                        UnsubscribeInputDevices();
                        _inputService = null;
                    }
                }

                _disposedValue = true;
            }
        }

        private void SubscribeInputDevices()
        {
            _keyboardListener.KeyPressed += KeyboardListener_KeyPressed;
            _keyboardListener.KeyReleased += KeyboardListener_KeyReleased;
            _keyboardListener.KeyTyped += KeyboardListener_KeyTyped;

            _mouseListener.MouseDown += MouseListener_MouseDown;
            _mouseListener.MouseUp += MouseListener_MouseUp;
            _mouseListener.MouseMoved += MouseListener_MouseMoved;
            _mouseListener.MouseWheelMoved += MouseListener_MouseWheelMoved;

            _gamePadListener.ButtonDown += GamePadListener_ButtonDown;
            _gamePadListener.ButtonUp += GamePadListener_ButtonUp;
        }

        private void UnsubscribeInputDevices()
        {
            _keyboardListener.KeyPressed -= KeyboardListener_KeyPressed;
            _keyboardListener.KeyReleased -= KeyboardListener_KeyReleased;
            _keyboardListener.KeyTyped -= KeyboardListener_KeyTyped;

            _mouseListener.MouseDown -= MouseListener_MouseDown;
            _mouseListener.MouseUp -= MouseListener_MouseUp;
            _mouseListener.MouseMoved -= MouseListener_MouseMoved;
            _mouseListener.MouseWheelMoved -= MouseListener_MouseWheelMoved;

            _gamePadListener.ButtonDown -= GamePadListener_ButtonDown;
            _gamePadListener.ButtonUp -= GamePadListener_ButtonUp;
        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {
            _inputReceiver.InjectKeyPress(e.Key);
        }

        private void KeyboardListener_KeyReleased(object sender, KeyboardEventArgs e)
        {
            _inputReceiver.InjectKeyRelease(e.Key);
        }

        private void KeyboardListener_KeyTyped(object sender, KeyboardEventArgs e)
        {
            _inputReceiver.InjectCharacter(e.Character.GetValueOrDefault());
        }

        private void MouseListener_MouseDown(object sender, MouseEventArgs e)
        {
            _inputReceiver.InjectMousePress(e.Button);
        }

        private void MouseListener_MouseUp(object sender, MouseEventArgs e)
        {
            _inputReceiver.InjectMouseRelease(e.Button);
        }

        private void MouseListener_MouseMoved(object sender, MouseEventArgs e)
        {
            _inputReceiver.InjectMouseMove(e.Position.X, e.Position.Y);
        }

        private void MouseListener_MouseWheelMoved(object sender, MouseEventArgs e)
        {
            _inputReceiver.InjectMouseWheel(e.ScrollWheelDelta);
        }

        private void GamePadListener_ButtonDown(object sender, GamePadEventArgs e)
        {
            if ((e.Button & Buttons.DPadUp) != 0)
                _inputReceiver.InjectCommand(Command.Up);
            else
            {
                if ((e.Button & Buttons.DPadDown) != 0)
                    _inputReceiver.InjectCommand(Command.Down);
                else
                {
                    if ((e.Button & Buttons.DPadLeft) != 0)
                        _inputReceiver.InjectCommand(Command.Left);
                    else
                    {
                        if ((e.Button & Buttons.DPadRight) != 0)
                            _inputReceiver.InjectCommand(Command.Right);
                        else
                            _inputReceiver.InjectButtonPress(e.Button);
                    }
                }
            }
        }

        private void GamePadListener_ButtonUp(object sender, GamePadEventArgs e)
        {
            _inputReceiver.InjectButtonRelease(e.Button);
        }

        /// <summary>Retrieves the input service from a service provider</summary>
        /// <param name="serviceProvider">
        ///     Service provider the service is taken from
        /// </param>
        /// <returns>The input service stored in the service provider</returns>
        private static IGuiInputService GetInputService(IServiceProvider serviceProvider)
        {
            var inputService = (IGuiInputService) serviceProvider.GetService(typeof(IGuiInputService));

            if (inputService == null)
            {
                throw new InvalidOperationException(
                    "Using the GUI with the DefaultInputCapturer requires the IInputService. " +
                    "Please either add the IInputService to Game.Services by using the " +
                    "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
                    "implementation for the GUI and assign it before GuiManager.Initialize() " +
                    "is called."
                );
            }

            return inputService;
        }

        /// <summary>Dummy receiver for input events</summary>
        private class DummyInputReceiver : IInputReceiver
        {
            /// <summary>Default instance of the dummy receiver</summary>
            public static readonly DummyInputReceiver Default = new DummyInputReceiver();

            /// <summary>Injects an input command into the input receiver</summary>
            /// <param name="command">Input command to be injected</param>
            public void InjectCommand(Command command)
            {
            }

            /// <summary>Called when a button on the gamepad has been pressed</summary>
            /// <param name="button">Button that has been pressed</param>
            public void InjectButtonPress(Buttons button)
            {
            }

            /// <summary>Called when a button on the gamepad has been released</summary>
            /// <param name="button">Button that has been released</param>
            public void InjectButtonRelease(Buttons button)
            {
            }

            /// <summary>Injects a mouse position update into the receiver</summary>
            /// <param name="x">New X coordinate of the mouse cursor on the screen</param>
            /// <param name="y">New Y coordinate of the mouse cursor on the screen</param>
            public void InjectMouseMove(float x, float y)
            {
            }

            /// <summary>Called when a mouse button has been pressed down</summary>
            /// <param name="button">Index of the button that has been pressed</param>
            public void InjectMousePress(MouseButton button)
            {
            }

            /// <summary>Called when a mouse button has been released again</summary>
            /// <param name="button">Index of the button that has been released</param>
            public void InjectMouseRelease(MouseButton button)
            {
            }

            /// <summary>Called when the mouse wheel has been rotated</summary>
            /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
            public void InjectMouseWheel(float ticks)
            {
            }

            /// <summary>Called when a key on the keyboard has been pressed down</summary>
            /// <param name="keyCode">Code of the key that was pressed</param>
            public void InjectKeyPress(Keys keyCode)
            {
            }

            /// <summary>Called when a key on the keyboard has been released again</summary>
            /// <param name="keyCode">Code of the key that was released</param>
            public void InjectKeyRelease(Keys keyCode)
            {
            }

            /// <summary>Handle user text input by a physical or virtual keyboard</summary>
            /// <param name="character">Character that has been entered</param>
            public void InjectCharacter(char character)
            {
            }
        }
    }
}