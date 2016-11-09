using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.NuclexGui
{
    public interface IGuiInputService
    {
        KeyboardListener KeyboardListener { get; }
        MouseListener MouseListener { get; }
        GamePadListener GamePadListener { get; }
        TouchListener TouchListener { get; }
    }

    public class GuiInputService : IGuiInputService
    {
        public GuiInputService(InputListenerComponent inputListener)
        {
            inputListener.Listeners.Add(KeyboardListener = new KeyboardListener());
            inputListener.Listeners.Add(MouseListener = new MouseListener());
            inputListener.Listeners.Add(GamePadListener = new GamePadListener());
            inputListener.Listeners.Add(TouchListener = new TouchListener());
        }

        public KeyboardListener KeyboardListener { get; }
        public MouseListener MouseListener { get; }
        public GamePadListener GamePadListener { get; }
        public TouchListener TouchListener { get; }
    }
}