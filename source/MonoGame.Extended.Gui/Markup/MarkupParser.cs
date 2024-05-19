using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Markup
{
    public class MarkupParser
    {
        public MarkupParser()
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
                {typeof(int), s => int.Parse(s)},
                {typeof(Color), s => s.StartsWith("#") ? ColorHelper.FromHex(s) : ColorHelper.FromName(s)}
            };

        private static object ConvertValue(Type propertyType, string input, object dataContext)
        {
            var value = ParseBinding(input, dataContext);

            if (_converters.TryGetValue(propertyType, out var converter))
                return converter(value); //property.SetValue(control, converter(value));

            if (propertyType.IsEnum)
                return
                    Enum.Parse(propertyType, value,
                        true); // property.SetValue(control, Enum.Parse(propertyType, value, true));

            throw new InvalidOperationException($"Converter not found for {propertyType}");
        }

        private static object ParseChildNode(XmlNode node, Control parent, object dataContext)
        {
            if (node is XmlText)
                return node.InnerText.Trim();

            if (_controlTypes.TryGetValue(node.Name, out var type))
            {
                var typeInfo = type.GetTypeInfo();
                var control = (Control) Activator.CreateInstance(type);

                // ReSharper disable once AssignNullToNotNullAttribute
                foreach (var attribute in node.Attributes.Cast<XmlAttribute>())
                {
                    var property = typeInfo.GetProperty(attribute.Name);

                    if (property != null)
                    {
                        var value = ConvertValue(property.PropertyType, attribute.Value, dataContext);
                        property.SetValue(control, value);
                    }
                    else
                    {
                        var parts = attribute.Name.Split('.');
                        var parentType = parts[0];
                        var propertyName = parts[1];
                        var propertyType = parent.GetAttachedPropertyType(propertyName);
                        var propertyValue = ConvertValue(propertyType, attribute.Value, dataContext);

                        if (!string.Equals(parent.GetType().Name, parentType, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException(
                                $"Attached properties are only supported on the immediate parent type {parentType}");

                        control.SetAttachedProperty(propertyName, propertyValue);
                    }
                }


                if (node.HasChildNodes)
                {
                    switch (control)
                    {
                        case ContentControl contentControl:
                            if (node.ChildNodes.Count > 1)
                                throw new InvalidOperationException("A content control can only have one child");

                            contentControl.Content = ParseChildNode(node.ChildNodes[0], control, dataContext);
                            break;
                        case LayoutControl layoutControl:
                            foreach (var childControl in ParseChildNodes(node.ChildNodes, control, dataContext))
                                layoutControl.Items.Add(childControl as Control);
                            break;
                    }
                }

                return control;
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

        private static IEnumerable<object> ParseChildNodes(XmlNodeList nodes, Control parent, object dataContext)
        {
            foreach (var node in nodes.Cast<XmlNode>())
            {
                if (node.Name == "xml")
                {
                    // TODO: Validate header
                }
                else
                {
                    yield return ParseChildNode(node, parent, dataContext);
                }
            }
        }

        public Control Parse(string filePath, object dataContext)
        {
            var d = new XmlDocument();
            d.Load(filePath);
            return ParseChildNodes(d.ChildNodes, null, dataContext)
                .LastOrDefault() as Control;
        }
    }
}
