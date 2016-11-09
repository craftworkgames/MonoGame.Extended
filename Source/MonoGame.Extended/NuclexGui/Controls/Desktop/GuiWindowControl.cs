using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>A window for hosting other controls</summary>
    public class GuiWindowControl : GuiDraggableControl
    {
        /// <summary>Initializes a new window control</summary>
        public GuiWindowControl() : base(true) { }

        /// <summary>Closes the window</summary>
        public void Close()
        {
            if (IsOpen)
            {
                Parent.Children.Remove(this);
            }
        }

        /// <summary>Whether the window is currently open</summary>
        public bool IsOpen
        {
            get { return Screen != null; }
        }

        /// <summary>Whether the window can be dragged with the mouse</summary>
        public new bool EnableDragging
        {
            get { return base.EnableDragging; }
            set { base.EnableDragging = value; }
        }

        /// <summary>Text in the title bar of the window</summary>
        public string Title;

    }
}
