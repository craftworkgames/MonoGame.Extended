using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Input.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        public abstract void Update(GameTime gameTime);
    }
}