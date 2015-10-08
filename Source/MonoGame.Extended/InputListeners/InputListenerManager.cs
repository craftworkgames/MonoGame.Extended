using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners
{
    public class InputListenerManager : IUpdate
    {
        public InputListenerManager()
        {
            _listeners = new List<InputListener>();
        }

        private readonly List<InputListener> _listeners;

        public IEnumerable<InputListener> Listeners
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
            var constructors = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(c => !c.GetParameters().Any())
                .ToArray();

            if (!constructors.Any())
                throw new InvalidOperationException(string.Format("No parameterless constructor defined for type {0}", typeof(T).Name));

            var listener = (T)constructors[0].Invoke(new object[0]);
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
