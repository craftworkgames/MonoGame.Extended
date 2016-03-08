using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGame.Extended.InputListeners
{
    public class TouchListener : InputListener
    {
        public event EventHandler<TouchEventArgs> TouchCancelled;
        public event EventHandler<TouchEventArgs> TouchEnded;
        public event EventHandler<TouchEventArgs> TouchMoved;

        public event EventHandler<TouchEventArgs> TouchStarted;

        internal TouchListener()
            : this(new TouchListenerSettings())
        {
        }

        // ReSharper disable once UnusedParameter.Local
        internal TouchListener(TouchListenerSettings settings)
        {
        }

        internal override void Update(GameTime gameTime)
        {
            var touchCollection = TouchPanel.GetState();

            foreach (var touchLocation in touchCollection)
            {
                switch (touchLocation.State)
                {
                    case TouchLocationState.Pressed:
                        TouchStarted.Raise(this, new TouchEventArgs(touchLocation));
                        break;
                    case TouchLocationState.Moved:
                        TouchMoved.Raise(this, new TouchEventArgs(touchLocation));
                        break;
                    case TouchLocationState.Released:
                        TouchEnded.Raise(this, new TouchEventArgs(touchLocation));
                        break;
                    case TouchLocationState.Invalid:
                        TouchCancelled.Raise(this, new TouchEventArgs(touchLocation));
                        break;
                }
            }
        }
    }
}