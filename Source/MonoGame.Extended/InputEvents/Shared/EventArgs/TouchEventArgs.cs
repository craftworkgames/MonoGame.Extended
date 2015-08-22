using System;
using Microsoft.Xna.Framework.Input.Touch;

namespace Microsoft.Xna.Framework.Input
{
	public class TouchEventArgs : EventArgs
    {
		public TouchLocation TouchLocation { get; private set; }

		internal TouchEventArgs (TouchLocation touchLocation)
		{
			TouchLocation = touchLocation;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null) { return false; }

			// If parameter cannot be cast to Point return false.
			TouchEventArgs t = obj as TouchEventArgs;
			if ((System.Object)t == null)
			{
				return false;
			}

			return TouchLocation.GetHashCode () == t.GetHashCode();
		}

		public override int GetHashCode()
		{
			return TouchLocation.Id;
		}
	}
}

