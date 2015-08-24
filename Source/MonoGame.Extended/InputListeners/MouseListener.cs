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
            _doubleClickMilliseconds = settings.DoubleClickMilliseconds;
            _dragThreshold = settings.DragThreshold;
        }

        private readonly int _doubleClickMilliseconds;
        private readonly int _dragThreshold;

        private MouseState _currentState;
        private MouseState _previousState;
        private GameTime _gameTime;
        private MouseEventArgs _previousClickArgs;
        private MouseEventArgs _mouseDownArgs;

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
                RaiseEvent(MouseDown, args);
                _mouseDownArgs = args;
            }
        }

        private void CheckButtonReleased(Func<MouseState, ButtonState> getButtonState, MouseButton button)
        {
            if (getButtonState(_currentState) == ButtonState.Released &&
                getButtonState(_previousState) == ButtonState.Pressed)
            {
                var args = new MouseEventArgs(_gameTime.TotalGameTime, _previousState, _currentState, button);

                RaiseEvent(MouseUp, args);

                if (_mouseDownArgs.Button == args.Button)
                {
                    var clickMovement = DistanceBetween(args.Position, _mouseDownArgs.Position);

                    // If the mouse hasn't moved much between mouse down and mouse up
                    if (clickMovement < _dragThreshold)
                    {
                        RaiseEvent(MouseClicked, args);

                        if (_previousClickArgs != null)
                        {
                            // If the last click was recent
                            var clickMilliseconds = (args.Time - _previousClickArgs.Time).TotalMilliseconds;

                            if (clickMilliseconds <= _doubleClickMilliseconds)
                                RaiseEvent(MouseDoubleClicked, args);

                            _previousClickArgs = null;
                        }
                    }
                    else // If the mouse has moved betweem mouse down and mouse up
                    {
                        RaiseEvent(MouseDragged, args);
                    }
                }

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
                RaiseEvent(MouseMoved, new MouseEventArgs(gameTime.TotalGameTime, _previousState, _currentState));

            // Handle mouse wheel events.
            if (_previousState.ScrollWheelValue != _currentState.ScrollWheelValue)
            {
                var value = _currentState.ScrollWheelValue / 120;
                var delta = (_currentState.ScrollWheelValue - _previousState.ScrollWheelValue) / 120;

                RaiseEvent(MouseWheelMoved, new MouseEventArgs(gameTime.TotalGameTime, _previousState, _currentState, MouseButton.None, value, delta));
            }

            _previousState = _currentState;
        }
     
        private static int DistanceBetween(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
