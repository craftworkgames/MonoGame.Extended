using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public class EventListenerManager
    {
        public EventListenerManager()
        {
            _listeners = new List<EventListener>();
        }

        private readonly List<EventListener> _listeners;

        public List<EventListener> Listeners
        {
            get { return _listeners; }
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var eventListener in _listeners)
                eventListener.Update(gameTime);
        }
    }
}
