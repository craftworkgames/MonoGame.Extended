using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.NuclexGui.Input
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
