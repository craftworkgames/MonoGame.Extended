using System;
using System.Collections.Generic;
using System.Reflection;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiJsonContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (type.GetTypeInfo().IsSubclassOf(typeof(GuiControl)))
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
    }
}