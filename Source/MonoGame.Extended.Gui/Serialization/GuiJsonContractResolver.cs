using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiJsonContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (type.GetTypeInfo().IsAbstract) //.IsSubclassOf(typeof(GuiControl)))
            {
                properties.Insert(0, new JsonProperty
                {
                    PropertyType = typeof(string),
                    PropertyName = "Type",
                    Readable = true,
                    Writable = false,
                    ValueProvider = new JsonShortTypeNameProvider()
                });
            }

            return properties;
        }

        private class JsonShortTypeNameProvider : IValueProvider
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
}