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

        public T CreateControl<T>(string skin, Vector2 position, string controlName = null, string text = null)
            where T : GuiControl, new()
        {
            var control = new T();
            _skin.Templates[skin].Apply(control);
            control.Name = controlName;
            control.Position = position;
            control.Text = text;
            return control;
        }
    }
}