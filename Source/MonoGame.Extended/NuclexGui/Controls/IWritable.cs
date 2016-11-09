namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>
    ///     Interface for controls that can be written into using the keyboard
    /// </summary>
    public interface IWritable : IFocusable
    {
        /// <summary>Title to be displayed in the on-screen keyboard</summary>
        string GuideTitle { get; }

        /// <summary>Description to be displayed in the on-screen keyboard</summary>
        string GuideDescription { get; }

        /// <summary>Text currently contained in the control</summary>
        /// <remarks>
        ///     Called before the on-screen keyboard is displayed to get the text currently
        ///     contained in the control and after the on-screen keyboard has been
        ///     acknowledged to assign the edited text to the control
        /// </remarks>
        string Text { get; set; }

        /// <summary>Called when the user has entered a character</summary>
        /// <param name="character">Character that has been entered</param>
        void OnCharacterEntered(char character);
    }
}