namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>Control used to represent the desktop</summary>
    public class GuiDesktopControl : GuiControl
    {
        /// <summary>Initializes a new control</summary>
        public GuiDesktopControl()
        {
        }

        /// <summary>True if the mouse is currently hovering over a GUI element</summary>
        public bool IsMouseOverGui
        {
            get
            {
                if (MouseOverControl == null)
                    return false;
                return !ReferenceEquals(MouseOverControl, this);
            }
        }

        /// <summary>Whether the GUI holds ownership of the input devices</summary>
        public bool IsInputCaptured
        {
            get
            {
                if (ActivatedControl == null)
                    return false;
                return !ReferenceEquals(ActivatedControl, this);
            }
        }
    }
}