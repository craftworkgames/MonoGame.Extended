namespace MonoGame.Extended.InputListeners
{
    public class KeyboardListenerSettings : InputListenerSettings<KeyboardListener>
    {
        public KeyboardListenerSettings()
        {
            InitialDelayMilliseconds = 800;
            RepeatDelayMilliseconds = 50;
        }

        public int InitialDelayMilliseconds { get; set; }
        public int RepeatDelayMilliseconds { get; set; }

        internal override KeyboardListener CreateListener()
        {
            return new KeyboardListener(this);
        }
    }
}