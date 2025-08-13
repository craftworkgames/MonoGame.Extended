// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization.Xml;

namespace MonoGame.Extended.Tests.Serialization.Xml;

public class XmlWriterExtensionsTests
{
    private static (XmlWriter writer, StringBuilder output) CreateXmlWriter()
    {
        StringBuilder output = new StringBuilder();

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;

        XmlWriter writer = XmlWriter.Create(output, settings);
        return (writer, output);
    }

    [Fact]
    public void WriteAttributeInt_WritesCorrectAttribute()
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        writer.WriteStartElement("test");
        writer.WriteAttributeInt("value", 1);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal("<test value=\"1\" />", output.ToString());
    }

    [Fact]
    public void WriteAttributeFloat_WritesCorrectAttribute()
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        writer.WriteStartElement("test");
        writer.WriteAttributeFloat("value", 1.2f);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal("<test value=\"1.2\" />", output.ToString());
    }

    [Theory]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    public void WriteAttributeBool_WritesCorrectAttribute(bool value, string expected)
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        writer.WriteStartElement("test");
        writer.WriteAttributeBool("value", value);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal($"<test value=\"{expected}\" />", output.ToString());
    }

    [Fact]
    public void WriteAttributeRectangle_WritesCorrectAttribute()
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        Rectangle rectangle = new Rectangle(1, 2, 3, 4);

        writer.WriteStartElement("test");
        writer.WriteAttributeRectangle("value", rectangle);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal($"<test value=\"{rectangle.X},{rectangle.Y},{rectangle.Width},{rectangle.Height}\" />", output.ToString());
    }

    [Fact]
    public void WriteAttributeVector2_WriteCorrectValue()
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        Vector2 vector = new Vector2(1.1f, 2.2f);

        writer.WriteStartElement("test");
        writer.WriteAttributeVector2("value", vector);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal($"<test value=\"{vector.X},{vector.Y}\" />", output.ToString());
    }

    [Fact]
    public void WriteAttributeVector3_WriteCorrectValue()
    {
        (XmlWriter writer, StringBuilder output) = CreateXmlWriter();

        Vector3 vector = new Vector3(1.1f, 2.2f, 3.3f);

        writer.WriteStartElement("test");
        writer.WriteAttributeVector3("value", vector);
        writer.WriteEndElement();
        writer.Flush();

        Assert.Equal($"<test value=\"{vector.X},{vector.Y},{vector.Z}\" />", output.ToString());
    }
}
