using System;
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

        public T AddListener<T>(EventListenerSettings<T> settings)
            where T : EventListener
        {
            var listener = settings.CreateListener();
            _listeners.Add(listener);
            return listener;
        }

        public T AddListener<T>()
            where T : EventListener
        {
            var listener = Activator.CreateInstance<T>();
            _listeners.Add(listener);
            return listener;
        }

        public void RemoveListener(EventListener listener)
        {
            _listeners.Remove(listener);
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var eventListener in _listeners)
                eventListener.Update(gameTime);
        }
    }
}
