using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using MonoGame.Extended.Gui.Controls;

namespace ContentExplorer
{
    public class GuiMarkupParser
    {
        public GuiMarkupParser()
        {
        }

        private static readonly Dictionary<string, Type> _controlTypes = new[]
        {
            typeof(Button),
            typeof(StackPanel),
            typeof(DockPanel),
            typeof(ToggleButton),
            typeof(ContentControl)
        }.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>
            {
                {typeof(object), s => s},
                {typeof(string), s => s},
                {typeof(bool), s => bool.Parse(s)},
                {typeof(int), s => int.Parse(s)}
            };

        private static object ParseChildNode(XmlNode node)
        {
            if (node is XmlText)
                return node.InnerText.Trim();

            if (_controlTypes.TryGetValue(node.Name, out var type))
            {
                var typeInfo = type.GetTypeInfo();
                var item = Activator.CreateInstance(type);

                foreach (var attribute in node.Attributes.Cast<XmlAttribute>())
                {
                    var property = typeInfo.GetProperty(attribute.Name);

                    if (property != null)
                    {
                        if (_converters.TryGetValue(property.PropertyType, out var converter))
                        {
                            var value = converter(attribute.Value);
                            property.SetValue(item, value);
                        }
                        else if (property.PropertyType.IsEnum)
                        {
                            var value = Enum.Parse(property.PropertyType, attribute.Value, true);
                            property.SetValue(item, value);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Converter not found for {property.PropertyType}");
                        }
                    }
                    else
                    {
                        // TODO: Attached properties
                    }
                }


                if (node.HasChildNodes)
                {
                    switch (item)
                    {
                        case ContentControl contentControl:
                            // TOOD: Throw if there's more than one child
                            contentControl.Content = ParseChildNode(node.ChildNodes[0]);
                            break;
                        case LayoutControl layoutControl:
                            foreach (var control in ParseChildNodes(node.ChildNodes))
                                layoutControl.Items.Add(control as Control);
                            break;
                    }
                }

                return item;
            }

            throw new InvalidOperationException($"Unknown control type {node.Name}");
        }

        private static IEnumerable<object> ParseChildNodes(XmlNodeList nodes)
        {
            foreach (var node in nodes.Cast<XmlNode>())
            {
                if (node.Name == "xml")
                {
                    // TODO: Validate header
                }
                else
                {
                    yield return ParseChildNode(node);
                }
            }
        }

        public Control Parse(string filePath)
        {
            var d = new XmlDocument();
            d.Load(filePath);
            return ParseChildNodes(d.ChildNodes).LastOrDefault() as Control;
        }
    }
}