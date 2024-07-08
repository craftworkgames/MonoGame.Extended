using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Input.InputListeners
{
    public class InputListenerComponent : GameComponent, IUpdateable
    {
        private readonly List<InputListener> _listeners;

        public InputListenerComponent(Game game)
            : base(game)
        {
            _listeners = new List<InputListener>();
        }

        public InputListenerComponent(Game game, params InputListener[] listeners)
            : base(game)
        {
            _listeners = new List<InputListener>(listeners);
        }

        public IList<InputListener> Listeners => _listeners;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Game.IsActive)
            {
                foreach (var listener in _listeners)
                    listener.Update(gameTime);
            }

            GamePadListener.CheckConnections();
        }
    }
}
