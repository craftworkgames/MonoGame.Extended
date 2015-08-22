using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class MouseEventListener : EventListener
    {
        public event EventHandler<MouseEventArgs> ButtonReleased;
        public event EventHandler<MouseEventArgs> ButtonPressed;
        public event EventHandler<MouseEventArgs> ButtonDoubleClicked;
        public event EventHandler<MouseEventArgs> MouseMoved;
        public event EventHandler<MouseEventArgs> MouseWheelMoved;

        private MouseState _previousState;
        private MouseEventArgs _lastClick;

        private readonly int _doubleClickMaxTimeDelta;
        private readonly int _doubleClickMaxMovementDelta;

        public MouseEventListener(int doubleClickMaxTimeDelta = 500, int doubleClickMaxMovementDelta = 2) // initial values are windows defaults
        {
            _doubleClickMaxTimeDelta = doubleClickMaxTimeDelta;
            _doubleClickMaxMovementDelta = doubleClickMaxMovementDelta;
            _lastClick = new MouseEventArgs(-1, -1, TimeSpan.Zero, Mouse.GetState(), Mouse.GetState());
        }

        public override void Update(GameTime gameTime)
        {
            var currentState = Mouse.GetState();
            
            // Check button press events.
            if (currentState.LeftButton == ButtonState.Pressed 
                && _previousState.LeftButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MouseEventArgs(currentState.X,currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.Left));
            }

            if (currentState.MiddleButton == ButtonState.Pressed 
                && _previousState.MiddleButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState,
                    currentState, MouseButton.Middle));
            }

            if (currentState.RightButton == ButtonState.Pressed 
                && _previousState.RightButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState,
                    currentState, MouseButton.Right));
            }

            if (currentState.XButton1 == ButtonState.Pressed
                && _previousState.XButton1 == ButtonState.Released) 
            {
                OnButtonPressed(this, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.XButton1));
            }

            if (currentState.XButton2 == ButtonState.Pressed
                && _previousState.XButton2 == ButtonState.Released) 
            {
                OnButtonPressed(this, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState,
                    currentState, MouseButton.XButton2));
            }

            // Check button releases.
            if (currentState.LeftButton == ButtonState.Released && _previousState.LeftButton == ButtonState.Pressed) 
            {
                RaiseEvent(ButtonReleased, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.Left));
            }

            if (currentState.MiddleButton == ButtonState.Released && _previousState.MiddleButton == ButtonState.Pressed) 
            {
                RaiseEvent(ButtonReleased, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState,
                    currentState, MouseButton.Middle));
            }

            if (currentState.RightButton == ButtonState.Released && _previousState.RightButton == ButtonState.Pressed) 
            {
                RaiseEvent(ButtonReleased, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.Right));
            }

            if (currentState.XButton1 == ButtonState.Released && _previousState.XButton1 == ButtonState.Pressed) 
            {
                RaiseEvent(ButtonReleased, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.XButton1));
            }

            if (currentState.XButton2 == ButtonState.Released && _previousState.XButton2 == ButtonState.Pressed) 
            {
                RaiseEvent(ButtonReleased, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.XButton2));
            }

            // Check for any sort of mouse movement. If a button is down, it's a drag,
            // otherwise it's a move.
            if (_previousState.X != currentState.X || _previousState.Y != currentState.Y)
            {
                RaiseEvent(MouseMoved, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState));
            }

            // Handle mouse wheel events.
            if (_previousState.ScrollWheelValue != currentState.ScrollWheelValue)
            {
                var value = currentState.ScrollWheelValue / 120;
                var delta = (currentState.ScrollWheelValue - _previousState.ScrollWheelValue) / 120;

                RaiseEvent(MouseWheelMoved, new MouseEventArgs(currentState.X, currentState.Y, gameTime.TotalGameTime, _previousState, 
                    currentState, MouseButton.None, value, delta));
            }

            _previousState = currentState;
        }
        
        private void OnButtonPressed(object sender, MouseEventArgs args)
        {
            // If this click is within the right time and position of the last click, 
            // raise a double-click event as well.           
            if (ButtonDoubleClicked != null && _lastClick.Button == args.Button && 
                (args.Time - _lastClick.Time).TotalMilliseconds < _doubleClickMaxTimeDelta &&
                DistanceBetween(args.Current, _lastClick.Current) < _doubleClickMaxMovementDelta)
            {
                ButtonDoubleClicked(sender, args);
                //args.Time = new TimeSpan(0);
            }
            else if (ButtonPressed != null) 
            {
                ButtonPressed(sender, args);
            }

            _lastClick = args;
        }   
     
        private static int DistanceBetween(MouseState a, MouseState b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
