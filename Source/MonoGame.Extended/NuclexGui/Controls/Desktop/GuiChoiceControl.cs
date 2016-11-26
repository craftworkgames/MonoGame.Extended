using System;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Control displaying an exclusive choice the user can select</summary>
    /// <remarks>
    ///     The choice control is equivalent to a radio button - if more than one
    ///     choice control is on a dialog, only one can be selected at a time.
    ///     To have several choice groups on a dialog, use panels to group them.
    /// </remarks>
    public class GuiChoiceControl : GuiPressableControl
    {
        /// <summary>Whether the choice is currently selected</summary>
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
            if (!Selected)
            {
                Selected = true;

                // Unselect all sibling choice controls in the same container
                UnselectSiblings();

                OnChanged();
            }
        }

        /// <summary>Triggers the changed event</summary>
        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        /// <summary>Disables all sibling choices on the same level</summary>
        private void UnselectSiblings()
        {
            // Disable any other choices in the same frame
            if (Parent != null)
            {
                var siblings = Parent.Children;
                for (var index = 0; index < siblings.Count; ++index)
                {
                    var control = siblings[index] as GuiChoiceControl;
                    if ((control != null) && (control != this) && control.Selected)
                    {
                        control.Selected = false;
                        control.OnChanged();
                    }
                }
            }
        }
    }
}