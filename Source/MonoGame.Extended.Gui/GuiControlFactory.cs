using System;
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

        public T Create<T>(string template, Vector2 position, string name = null, string text = null)
            where T : GuiControl, new()
        {
            var control = new T();
            _skin.GetStyle(template).Apply(control);
            control.Name = name;
            control.Position = position;
            control.Text = text;
            return control;
        }

        public T Create<T>(string template, Action<T> onCreate)
            where T : GuiControl, new()
        {
            var control = new T();
            _skin.GetStyle(template).Apply(control);
            onCreate(control);
            return control;
        }

        public GuiControl Create(Type type, string template)
        {
            var control = (GuiControl) Activator.CreateInstance(type);
            _skin.GetStyle(template).Apply(control);
            return control;
        }
    }
}