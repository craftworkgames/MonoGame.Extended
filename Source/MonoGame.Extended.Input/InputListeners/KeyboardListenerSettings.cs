namespace MonoGame.Extended.Input.InputListeners
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

        public override KeyboardListener CreateListener()
        {
            return new KeyboardListener(this);
        }
    }
}