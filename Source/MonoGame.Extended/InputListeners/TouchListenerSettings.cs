namespace MonoGame.Extended.InputListeners
{
    public class TouchListenerSettings : InputListenerSettings<TouchListener>
    {
        public TouchListenerSettings()
        {
        }

        internal override TouchListener CreateListener()
        {
            return new TouchListener();
        }
    }
}