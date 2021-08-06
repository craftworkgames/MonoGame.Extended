namespace MonoGame.Extended.Input.InputListeners
{
    public abstract class InputListenerSettings<T>
        where T : InputListener
    {
        public abstract T CreateListener();
    }
}