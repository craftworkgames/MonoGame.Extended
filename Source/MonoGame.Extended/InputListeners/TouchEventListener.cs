using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGame.Extended.InputListeners
{
    public class TouchEventListener : EventListener
    {
		internal event EventHandler<TouchEventArgs> TouchStarted;
        internal event EventHandler<TouchEventArgs> TouchEnded;
        internal event EventHandler<TouchEventArgs> TouchMoved;
		internal event EventHandler<TouchEventArgs> TouchCancelled;
        
		public override void Update(GameTime gameTime)
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

