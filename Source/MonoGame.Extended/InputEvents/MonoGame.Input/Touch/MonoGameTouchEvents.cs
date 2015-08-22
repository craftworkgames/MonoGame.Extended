using System;
using Microsoft.Xna.Framework.Input.Touch;

namespace Microsoft.Xna.Framework.Input
{
	public class MonoGameTouchEvents
	{
		internal event EventHandler<TouchEventArgs> TouchBegan;
		internal event EventHandler<TouchEventArgs> TouchMoved;
		internal event EventHandler<TouchEventArgs> TouchEnded;
		internal event EventHandler<TouchEventArgs> TouchCancelled;


		public void Update(GameTime gameTime)
		{
			TouchCollection touchCollection = TouchPanel.GetState();
			foreach (TouchLocation tl in touchCollection) 
			{
				switch (tl.State)
				{
					case TouchLocationState.Pressed:
					OnTouchBegan(this, new TouchEventArgs(tl));
						break;
					case TouchLocationState.Moved:
						OnTouchMoved(this, new TouchEventArgs(tl));
						break;
					case TouchLocationState.Released:
						OnTouchEnded(this, new TouchEventArgs(tl));
						break;
					case TouchLocationState.Invalid:
						OnTouchCancelled(this, new TouchEventArgs(tl));
						break;
				}
			}
		}			

		private void OnTouchBegan(object sender, TouchEventArgs args)
		{
			if (TouchBegan != null) 
			{
				TouchBegan(sender, args);
			}
		}

		private void OnTouchMoved(object sender, TouchEventArgs args)
		{
			if (TouchMoved != null) 
			{
				TouchMoved(sender, args);
			}
		}

		private void OnTouchEnded(object sender, TouchEventArgs args)
		{
			if (TouchEnded != null) 
			{
				TouchEnded(sender, args);
			}
		}

		private void OnTouchCancelled(object sender, TouchEventArgs args)
		{
			if (TouchCancelled != null) 
			{
				TouchCancelled(sender, args);
			}
		}
	}
}

