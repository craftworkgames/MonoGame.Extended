using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.InputListeners
{
    public class InputListenerManager : IUpdate
    {
        public InputListenerManager()
            : this(null)
        {
        }

        public InputListenerManager(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;
            _listeners = new List<InputListener>();
        }

        private readonly ViewportAdapter _viewportAdapter;
        private readonly List<InputListener> _listeners;

        public IEnumerable<InputListener> Listeners => _listeners;

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
                .GetTypeInfo().DeclaredConstructors
                .Where(c => !c.GetParameters().Any())
                .ToArray();

            if (!constructors.Any())
                throw new InvalidOperationException($"No parameterless constructor defined for type {typeof (T).Name}");

            var listener = (T)constructors[0].Invoke(new object[0]);
            listener.ViewportAdapter = _viewportAdapter;
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

            GamePadListener.CheckConnections();
        }
    }
}
