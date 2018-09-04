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

        private static readonly Dictionary<string, Type> _controlTypes = 
                typeof(Control).Assembly
                    .ExportedTypes
                    .Where(t => t.IsSubclassOf(typeof(Control)))
                    .ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>
            {
                {typeof(object), s => s},
                {typeof(string), s => s},
                {typeof(bool), s => bool.Parse(s)},
                {typeof(int), s => int.Parse(s)}
            };

        private static object ParseChildNode(XmlNode node, object dataContext)
        {
            if (node is XmlText)
                return node.InnerText.Trim();

            if (_controlTypes.TryGetValue(node.Name, out var type))
            {
                var typeInfo = type.GetTypeInfo();
                var item = Activator.CreateInstance(type);

                // ReSharper disable once AssignNullToNotNullAttribute
                foreach (var attribute in node.Attributes.Cast<XmlAttribute>())
                {
                    var property = typeInfo.GetProperty(attribute.Name);

                    if (property != null)
                    {
                        var value = ParseBinding(attribute.Value, dataContext);

                        if (_converters.TryGetValue(property.PropertyType, out var converter))
                            property.SetValue(item, converter(value));
                        else if (property.PropertyType.IsEnum)
                            property.SetValue(item, Enum.Parse(property.PropertyType, value, true));
                        else
                            throw new InvalidOperationException($"Converter not found for {property.PropertyType}");
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
                            contentControl.Content = ParseChildNode(node.ChildNodes[0], dataContext);
                            break;
                        case LayoutControl layoutControl:
                            foreach (var control in ParseChildNodes(node.ChildNodes, dataContext))
                                layoutControl.Items.Add(control as Control);
                            break;
                    }
                }

                return item;
            }

            throw new InvalidOperationException($"Unknown control type {node.Name}");
        }

        private static string ParseBinding(string expression, object dataContext)
        {
            if (dataContext != null && expression.StartsWith("{{") && expression.EndsWith("}}"))
            {
                var binding = expression.Substring(2, expression.Length - 4);
                var bindingValue = dataContext
                    .GetType()
                    .GetProperty(binding)
                    ?.GetValue(dataContext);

                return $"{bindingValue}";
            }

            return expression;
        }

        private static IEnumerable<object> ParseChildNodes(XmlNodeList nodes, object dataContext)
        {
            foreach (var node in nodes.Cast<XmlNode>())
            {
                if (node.Name == "xml")
                {
                    // TODO: Validate header
                }
                else
                {
                    yield return ParseChildNode(node, dataContext);
                }
            }
        }

        public Control Parse(string filePath, object dataContext)
        {
            var d = new XmlDocument();
            d.Load(filePath);
            return ParseChildNodes(d.ChildNodes, dataContext)
                .LastOrDefault() as Control;
        }
    }
}