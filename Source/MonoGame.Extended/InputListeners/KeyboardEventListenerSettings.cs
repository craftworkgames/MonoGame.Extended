namespace MonoGame.Extended.InputListeners
{
    public class KeyboardEventListenerSettings : EventListenerSettings<KeyboardEventListener>
    {
        public KeyboardEventListenerSettings()
        {
            InitialDelayMilliseconds = 800;
            RepeatDelayMilliseconds = 50;
        }

        public int InitialDelayMilliseconds { get; set; }
        public int RepeatDelayMilliseconds { get; set; }

        internal override KeyboardEventListener CreateListener()
        {
            return new KeyboardEventListener(InitialDelayMilliseconds, RepeatDelayMilliseconds);
        }
    }
}