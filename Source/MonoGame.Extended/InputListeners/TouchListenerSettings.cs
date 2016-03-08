namespace MonoGame.Extended.InputListeners
{
    public class TouchListenerSettings : InputListenerSettings<TouchListener>
    {
        internal override TouchListener CreateListener()
        {
            return new TouchListener();
        }
    }
}