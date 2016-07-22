using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        //public ViewportAdapter ViewportAdapter { get; set; }

        public abstract void Update(GameTime gameTime);
    }
}