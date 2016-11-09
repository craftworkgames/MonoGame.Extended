using System;

namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>Event argument class that carries a control instance</summary>
    public class ControlEventArgs : EventArgs
    {
        /// <summary>Control that will be accessible to the event subscribers</summary>
        private readonly GuiControl _control;

        /// <summary>Initializes a new control event args instance</summary>
        /// <param name="control">Control to provide to the subscribers of the event</param>
        public ControlEventArgs(GuiControl control)
        {
            _control = control;
        }

        /// <summary>Control that has been provided for the event</summary>
        public GuiControl Control => _control;
    }
}