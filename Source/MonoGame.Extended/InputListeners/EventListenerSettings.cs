namespace MonoGame.Extended.InputListeners
{
    public abstract class EventListenerSettings<T>
        where T : EventListener
    {
        internal abstract T CreateListener();
    }
}