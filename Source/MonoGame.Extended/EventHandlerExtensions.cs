using System;

namespace MonoGame.Extended
{
    internal static class EventHandlerExtensions
    {
        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T args)
            where T : EventArgs
        {
            var handler = eventHandler;

            if (handler != null)
                handler(sender, args);
        }

        public static void Raise(this EventHandler eventHandler, object sender, EventArgs args)
        {
            var handler = eventHandler;

            if (handler != null)
                handler(sender, args);
        }
    }
}