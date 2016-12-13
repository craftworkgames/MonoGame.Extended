using Newtonsoft.Json.Serialization;

namespace MonoGame.Extended.Gui.Serialization
{
    public class JsonShortTypeNameProvider : IValueProvider
    {
        public JsonShortTypeNameProvider()
        {
        }

        public void SetValue(object target, object value)
        {
        }

        public object GetValue(object target)
        {
            return target.GetType().Name;
        }
    }
}