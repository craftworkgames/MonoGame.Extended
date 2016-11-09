namespace MonoGame.Extended.NuclexGui.Input
{
    /// <summary>
    ///     Interface for input capturers that monitor user input and forward it to
    ///     a freely settable input receiver
    /// </summary>
    public interface IInputCapturer
    {
        /// <summary>Input receiver any captured input will be sent to</summary>
        IInputReceiver InputReceiver { get; set; }
    }
}