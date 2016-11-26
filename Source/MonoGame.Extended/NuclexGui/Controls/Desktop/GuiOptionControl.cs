using System;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Control displaying an option the user can toggle on and off</summary>
    public class GuiOptionControl : GuiPressableControl
    {
        /// <summary>Whether the option is currently selected</summary>
        public bool Selected;

        /// <summary>Text that will be shown on the button</summary>
        public string Text;

        /// <summary>Determines where text or image will be shown relative to control</summary>
        public GuiPressableDescriptionPosition DescriptionPosition;

        /// <summary>Will be triggered when the choice is changed</summary>
        public event EventHandler Changed;

        /// <summary>Called when the button is pressed</summary>
        protected override void OnPressed()
        {
            Selected = !Selected;
            OnChanged();
        }

        /// <summary>Triggers the changed event</summary>
        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }
    }
}