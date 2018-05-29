namespace MonoGame.Extended.Input.InputListeners
{
    public interface IInputService
    {
        KeyboardListener GuiKeyboardListener { get; }

        MouseListener GuiMouseListener { get; }

        GamePadListener GuiGamePadListener { get; }

        TouchListener GuiTouchListener { get; }
    }
}
