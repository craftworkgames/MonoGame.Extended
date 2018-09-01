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
            typeof(ToggleButton)
        }.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

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
                        // TODO: handle all the different property types
                        if (property.PropertyType == typeof(bool))
                            property.SetValue(item, bool.Parse(attribute.Value));
                        else
                            property.SetValue(item, attribute.Value);
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