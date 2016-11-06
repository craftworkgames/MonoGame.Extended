namespace MonoGame.Extended.InputListeners
{
    public interface IInputService
    {
        KeyboardListener KeyboardListener { get; }

        MouseListener MouseListener { get; }

        GamePadListener GamePadListener { get; }

        TouchListener TouchListener { get; }

        void Update();
    }
}
