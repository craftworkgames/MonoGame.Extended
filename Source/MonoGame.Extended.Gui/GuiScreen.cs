using System.Linq;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiScreen
    {
        public GuiScreen()
        {
            Controls = new GuiControlCollection();
        }

        public GuiControlCollection Controls { get; }

        public T FindControl<T>(string name)
            where T : GuiControl
        {
            return FindControl<T>(Controls, name) as T;
        }

        private static T FindControl<T>(GuiControlCollection controls, string name)
            where T : GuiControl
        {
            foreach (var control in controls)
            {
                if (control is T && control.Name == name)
                    return control as T;

                return FindControl<T>(control.Children, name);
            }

            return null;
        }
    }
}