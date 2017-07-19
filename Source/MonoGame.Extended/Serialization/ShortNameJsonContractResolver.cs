using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MonoGame.Extended.Serialization
{
    public class ShortNameJsonContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization)
                .Where(p => p.Writable || IsListProperty(p))
                .ToList();

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsAbstract)
            {
                properties.Insert(0, new JsonProperty
                {
                    PropertyType = typeof(string),
                    PropertyName = "type",
                    Readable = true,
                    Writable = false,
                    ValueProvider = new JsonShortTypeNameProvider()
                });
            }

            return properties;
        }

        private static bool IsListProperty(JsonProperty property)
        {
            return typeof(IList).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo());
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