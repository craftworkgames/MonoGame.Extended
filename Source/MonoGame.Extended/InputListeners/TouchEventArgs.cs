using System;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGame.Extended.InputListeners
{
    public class TouchEventArgs : EventArgs
    {
        public TouchLocation Location { get; }

        internal TouchEventArgs(TouchLocation location)
        {
            Location = location;
        }

        public override bool Equals(object other)
        {
            var args = other as TouchEventArgs;

            if (args == null)
            {
                return false;
            }

            return ReferenceEquals(this, args) || Location.Id.Equals(args.Location.Id);
        }

        public override int GetHashCode()
        {
            return Location.Id.GetHashCode();
        }
    }
}