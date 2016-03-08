namespace MonoGame.Extended.InputListeners
{
    public class KeyboardListenerSettings : InputListenerSettings<KeyboardListener>
    {
        public int InitialDelayMilliseconds { get; set; }
        public int RepeatDelayMilliseconds { get; set; }

        public KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 800;
            RepeatDelayMilliseconds = 50;
        }

        internal override KeyboardListener CreateListener()
        {
            return new KeyboardListener(this);
        }
    }
}