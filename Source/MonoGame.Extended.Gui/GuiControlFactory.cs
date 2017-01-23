using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiControlFactory
    {
        private readonly GuiSkin _skin;

        public GuiControlFactory(GuiSkin skin)
        {
            _skin = skin;
        }

        public T CreateControl<T>(string skinName, Vector2 position, string controlName, string text = null)
            where T : GuiControl, new()
        {
            var control = new T();
            _skin.Templates[skinName].Apply(control);
            control.Name = controlName;
            control.Position = position;
            control.Text = text;
            return control;
        }
    }
}