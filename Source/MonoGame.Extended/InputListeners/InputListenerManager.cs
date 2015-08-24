using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public class InputListenerManager
    {
        public InputListenerManager()
        {
            _listeners = new List<InputListener>();
        }

        private readonly List<InputListener> _listeners;

        public List<InputListener> Listeners
        {
            get { return _listeners; }
        }

        public T AddListener<T>(InputListenerSettings<T> settings)
            where T : InputListener
        {
            var listener = settings.CreateListener();
            _listeners.Add(listener);
            return listener;
        }

        public T AddListener<T>()
            where T : InputListener
        {
            var listener = Activator.CreateInstance<T>();
            _listeners.Add(listener);
            return listener;
        }

        public void RemoveListener(InputListener listener)
        {
            _listeners.Remove(listener);
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var listener in _listeners)
                listener.Update(gameTime);
        }
    }
}
