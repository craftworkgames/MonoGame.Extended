using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGame.Extended.InputListeners
{
    public class TouchListener : InputListener
    {
        internal TouchListener()
            : this(new TouchListenerSettings())
        {
        }

        // ReSharper disable once UnusedParameter.Local
        internal TouchListener(TouchListenerSettings settings)
        {
        }

		public event EventHandler<TouchEventArgs> TouchStarted;
        public event EventHandler<TouchEventArgs> TouchEnded;
        public event EventHandler<TouchEventArgs> TouchMoved;
        public event EventHandler<TouchEventArgs> TouchCancelled;

        internal override void Update(GameTime gameTime)
		{
			var touchCollection = TouchPanel.GetState();

			foreach (var touchLocation in touchCollection) 
			{
				switch (touchLocation.State)
				{
					case TouchLocationState.Pressed:
                        RaiseEvent(TouchStarted, new TouchEventArgs(touchLocation));
						break;
					case TouchLocationState.Moved:
                        RaiseEvent(TouchMoved, new TouchEventArgs(touchLocation));
						break;
					case TouchLocationState.Released:
                        RaiseEvent(TouchEnded, new TouchEventArgs(touchLocation));
						break;
					case TouchLocationState.Invalid:
                        RaiseEvent(TouchCancelled, new TouchEventArgs(touchLocation));
						break;
				}
			}
		}			
	}
}
