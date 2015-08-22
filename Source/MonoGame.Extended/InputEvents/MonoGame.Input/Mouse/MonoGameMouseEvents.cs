using System;

namespace Microsoft.Xna.Framework.Input
{
    internal class MonoGameMouseEvents
    {
        internal event EventHandler<MouseEventArgs> ButtonReleased;
        internal event EventHandler<MouseEventArgs> ButtonPressed;
        internal event EventHandler<MouseEventArgs> ButtonDoubleClicked;
        internal event EventHandler<MouseEventArgs> MouseMoved;
        internal event EventHandler<MouseEventArgs> MouseWheelMoved;

        private MouseState _previous;
        private MonoGameMouseEventArgs _lastClick;

        private readonly int _doubleClickMaxTimeDelta;
        private readonly int _doubleClickMaxMovementDelta;

        internal MonoGameMouseEvents(int doubleClickMaxTimeDelta, int doubleClickMaxMovementDelta)
        {
            _doubleClickMaxTimeDelta = doubleClickMaxTimeDelta;
            _doubleClickMaxMovementDelta = doubleClickMaxMovementDelta;

            _lastClick = new MonoGameMouseEventArgs(
                -1, 
                -1,
                new TimeSpan(),
                Mouse.GetState(),
                Mouse.GetState());
        }

        internal void Update(GameTime gameTime)
        {
            var current = Mouse.GetState();
            
            // Check button press events.
            if (current.LeftButton == ButtonState.Pressed 
                && _previous.LeftButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MonoGameMouseEventArgs(
                    current.X,
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous, 
                    current,
                    MouseButton.Left));
            }

            if (current.MiddleButton == ButtonState.Pressed 
                && _previous.MiddleButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MonoGameMouseEventArgs(
                    current.X,
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous,
                    current,
                    MouseButton.Middle));
            }

            if (current.RightButton == ButtonState.Pressed 
                && _previous.RightButton == ButtonState.Released) 
            {
                OnButtonPressed(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous,
                    current, 
                    MouseButton.Right));
            }

            if (current.XButton1 == ButtonState.Pressed
                && _previous.XButton1 == ButtonState.Released) 
            {
                OnButtonPressed(this, new MonoGameMouseEventArgs(
                    current.X,
                    current.Y,
                    gameTime.TotalGameTime,
                    _previous, 
                    current,
                    MouseButton.XButton1));
            }

            if (current.XButton2 == ButtonState.Pressed
                && _previous.XButton2 == ButtonState.Released) 
            {
                OnButtonPressed(this, new MonoGameMouseEventArgs(
                    current.X,
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous,
                    current,
                    MouseButton.XButton2));
            }

            // Check button releases.
            if (current.LeftButton == ButtonState.Released 
                && _previous.LeftButton == ButtonState.Pressed) 
            {
                OnButtonReleased(this, new MonoGameMouseEventArgs(
                    current.X,
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous,
                    current, 
                    MouseButton.Left));
            }

            if (current.MiddleButton == ButtonState.Released
                && _previous.MiddleButton == ButtonState.Pressed) 
            {
                OnButtonReleased(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime,
                    _previous,
                    current,
                    MouseButton.Middle));
            }

            if (current.RightButton == ButtonState.Released
                && _previous.RightButton == ButtonState.Pressed) 
            {
                OnButtonReleased(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime, 
                    _previous, 
                    current, 
                    MouseButton.Right));
            }

            if (current.XButton1 == ButtonState.Released
                && _previous.XButton1 == ButtonState.Pressed) 
            {
                OnButtonReleased(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime, 
                    _previous, 
                    current, 
                    MouseButton.XButton1));
            }

            if (current.XButton2 == ButtonState.Released
                && _previous.XButton2 == ButtonState.Pressed) 
            {
                OnButtonReleased(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime, 
                    _previous, 
                    current, 
                    MouseButton.XButton2));
            }

            // Check for any sort of mouse movement. If a button is down, it's a drag,
            // otherwise it's a move.
            if (_previous.X != current.X || _previous.Y != current.Y)
            {
                OnMouseMoved(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime, 
                    _previous, 
                    current));
            }

            // Handle mouse wheel events.
            if (_previous.ScrollWheelValue != current.ScrollWheelValue)
            {
                var value = current.ScrollWheelValue / 120;
                var delta = (current.ScrollWheelValue - _previous.ScrollWheelValue) / 120;
                OnMouseWheelMoved(this, new MonoGameMouseEventArgs(
                    current.X, 
                    current.Y, 
                    gameTime.TotalGameTime, 
                    _previous, 
                    current, 
                    MouseButton.None, 
                    value, 
                    delta));
            }

            _previous = current;
        }

        private void OnButtonReleased(object sender, MonoGameMouseEventArgs args)
        {
            if (ButtonReleased != null) 
            {
                ButtonReleased(sender, args);
            }
        }

        private void OnButtonPressed(object sender, MonoGameMouseEventArgs args)
        {
            // If this click is within the right time and position of the last click, 
            // raise a double-click event as well.           
            if (ButtonDoubleClicked != null &&
                _lastClick.Button == args.Button &&
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

        private void OnMouseMoved(object sender, MouseEventArgs args)
        {
            if (MouseMoved != null) 
            {
                MouseMoved(sender, args);
            }
        }

        private void OnMouseWheelMoved(object sender, MouseEventArgs args)
        {
            if (MouseWheelMoved != null) 
            {
                MouseWheelMoved(sender, args);
            }
        }        
    }
}
