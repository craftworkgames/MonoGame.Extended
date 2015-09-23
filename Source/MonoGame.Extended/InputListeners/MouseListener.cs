using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class MouseListener : InputListener
    {
        internal MouseListener()
            : this(new MouseListenerSettings())
        {
        }

        internal MouseListener(MouseListenerSettings settings)
        {
            DoubleClickMilliseconds = settings.DoubleClickMilliseconds;
            DragThreshold = settings.DragThreshold;
        }

        public int DoubleClickMilliseconds { get; private set; }
        public int DragThreshold { get; private set; }

        private MouseState _currentState;
        private MouseState _previousState;
        private GameTime _gameTime;
        private MouseEventArgs _previousClickArgs;
        private MouseEventArgs _mouseDownArgs;
        private bool _hasDoubleClicked;

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseClicked;
        public event EventHandler<MouseEventArgs> MouseDragged;
        public event EventHandler<MouseEventArgs> MouseDoubleClicked;
        public event EventHandler<MouseEventArgs> MouseMoved;
        public event EventHandler<MouseEventArgs> MouseWheelMoved;

        private void CheckButtonPressed(Func<MouseState, ButtonState> getButtonState, MouseButton button)
        {
            if (getButtonState(_currentState) == ButtonState.Pressed && 
                getButtonState(_previousState) == ButtonState.Released)
            {
                var args = new MouseEventArgs(_gameTime.TotalGameTime, _previousState, _currentState, button);

                MouseDown.Raise(this, args);
                _mouseDownArgs = args;

                if (_previousClickArgs != null)
                {
                    // If the last click was recent
                    var clickMilliseconds = (args.Time - _previousClickArgs.Time).TotalMilliseconds;

                    if (clickMilliseconds <= DoubleClickMilliseconds)
                    {
                        MouseDoubleClicked.Raise(this, args);
                        _hasDoubleClicked = true;
                    }

                    _previousClickArgs = null;
                }
            }
        }

        private void CheckButtonReleased(Func<MouseState, ButtonState> getButtonState, MouseButton button)
        {
            if (getButtonState(_currentState) == ButtonState.Released &&
                getButtonState(_previousState) == ButtonState.Pressed)
            {
                var args = new MouseEventArgs(_gameTime.TotalGameTime, _previousState, _currentState, button);
                
                if (_mouseDownArgs.Button == args.Button)
                {
                    var clickMovement = DistanceBetween(args.Position, _mouseDownArgs.Position);

                    // If the mouse hasn't moved much between mouse down and mouse up
                    if (clickMovement < DragThreshold)
                    {
                        if(!_hasDoubleClicked)
                            MouseClicked.Raise(this, args);
                    }
                    else // If the mouse has moved between mouse down and mouse up
                    {
                        MouseDragged.Raise(this, args);
                    }
                }

                MouseUp.Raise(this, args);

                _hasDoubleClicked = false;
                _previousClickArgs = args;
            }
        }

        internal override void Update(GameTime gameTime)
        {
            _gameTime = gameTime;
            _currentState = Mouse.GetState();
            
            CheckButtonPressed(s => s.LeftButton, MouseButton.Left);
            CheckButtonPressed(s => s.MiddleButton, MouseButton.Middle);
            CheckButtonPressed(s => s.RightButton, MouseButton.Right);
            CheckButtonPressed(s => s.XButton1, MouseButton.XButton1);
            CheckButtonPressed(s => s.XButton2, MouseButton.XButton2);

            CheckButtonReleased(s => s.LeftButton, MouseButton.Left);
            CheckButtonReleased(s => s.MiddleButton, MouseButton.Middle);
            CheckButtonReleased(s => s.RightButton, MouseButton.Right);
            CheckButtonReleased(s => s.XButton1, MouseButton.XButton1);
            CheckButtonReleased(s => s.XButton2, MouseButton.XButton2);

            // Check for any sort of mouse movement. 
            if (_previousState.X != _currentState.X || _previousState.Y != _currentState.Y)
                MouseMoved.Raise(this, new MouseEventArgs(gameTime.TotalGameTime, _previousState, _currentState));

            // Handle mouse wheel events.
            if (_previousState.ScrollWheelValue != _currentState.ScrollWheelValue)
                MouseWheelMoved.Raise(this, new MouseEventArgs(gameTime.TotalGameTime, _previousState, _currentState));

            _previousState = _currentState;
        }
     
        private static int DistanceBetween(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
