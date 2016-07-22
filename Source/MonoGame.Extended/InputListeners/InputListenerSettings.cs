namespace MonoGame.Extended.InputListeners
{
    public abstract class InputListenerSettings<T>
        where T : InputListener
    {
        public abstract T CreateListener();
    }
}