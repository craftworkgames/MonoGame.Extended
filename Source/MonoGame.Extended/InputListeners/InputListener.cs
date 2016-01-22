using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        internal ViewportAdapter ViewportAdapter { get; set; }

        internal abstract void Update(GameTime gameTime);
    }
}