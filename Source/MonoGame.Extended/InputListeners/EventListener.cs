using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public abstract class EventListener
    {
        protected void RaiseEvent<T>(EventHandler<T> eventHandler, T args)
            where T : EventArgs
        {
            var handler = eventHandler;

            if (handler != null)
                handler(this, args);
        }

        public abstract void Update(GameTime gameTime);
    }
}