namespace MonoGame.Extended.InputListeners
{
    public class TouchEventListenerSettings : EventListenerSettings<TouchEventListener>
    {
        internal override TouchEventListener CreateListener()
        {
            return new TouchEventListener();
        }
    }
}