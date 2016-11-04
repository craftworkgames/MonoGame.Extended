namespace MonoGame.Extended.InputListeners
{
    public class TouchListenerSettings : InputListenerSettings<TouchListener>
    {
        public TouchListenerSettings()
        {
        }

        public override TouchListener CreateListener()
        {
            return new TouchListener();
        }
    }
}