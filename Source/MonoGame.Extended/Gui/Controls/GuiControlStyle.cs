using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiControlStyle
    {
        public GuiControlStyle(Type targetType)
        {
            TargetType = targetType;
            Setters = new Dictionary<string, object>();
        }

        public Type TargetType { get; }
        public Dictionary<string, object> Setters { get; set; }

        public void Apply(GuiControl control)
        {
            foreach (var propertyName in Setters.Keys)
            {
                var propertyInfo = TargetType.GetRuntimeProperty(propertyName);
                var value = Setters[propertyName];
                propertyInfo.SetValue(control, value);
            }
        }
    }
}