using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public sealed class EventListenerManager
    {
        private readonly List<EventListener> _listeners;

        public List<EventListener> Listeners
        {
            get { return _listeners; }
        } 

        public EventListenerManager()
        {
            _listeners = new List<EventListener>();
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var eventListener in _listeners)
                eventListener.Update(gameTime);
        }
    }
}
