using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        internal abstract void Update(GameTime gameTime);
    }
}