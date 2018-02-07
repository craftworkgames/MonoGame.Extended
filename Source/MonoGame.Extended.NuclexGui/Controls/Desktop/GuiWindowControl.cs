namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>A window for hosting other controls</summary>
    public class GuiWindowControl : GuiResizableControl
    {
        /// <summary>Text in the title bar of the window</summary>
        public string Title;

        /// <summary>Initializes a new window control</summary>
        public GuiWindowControl() : base(true)
        {
        }

        /// <summary>Whether the window is currently open</summary>
        public bool IsOpen => Screen != null;

        /// <summary>Whether the window can be dragged with the mouse</summary>
        public new bool EnableDragging
        {
            get { return base.EnableDragging; }
            set { base.EnableDragging = value; }
        }

        /// <summary>Whether the window can be resized with the mouse</summary>
        public new bool EnableResizing
        {
            get { return base.EnableResizing; }
            set { base.EnableResizing = value; }
        }

        /// <summary>Closes the window</summary>
        public void Close()
        {
            if (IsOpen)
                Parent.Children.Remove(this);
        }
    }
}