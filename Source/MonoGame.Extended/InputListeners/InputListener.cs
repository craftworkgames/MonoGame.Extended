using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        protected void RaiseEvent<T>(EventHandler<T> eventHandler, T args)
            where T : EventArgs
        {
            var handler = eventHandler;

            if (handler != null)
                handler(this, args);
        }

        internal abstract void Update(GameTime gameTime);
    }
}