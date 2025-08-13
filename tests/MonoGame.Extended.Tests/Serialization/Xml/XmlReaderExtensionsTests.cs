// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization.Xml;

namespace MonoGame.Extended.Tests.Serialization.Xml;

public class XmlReaderExtensionsTests
{
    private static XmlReader CreateXmlReader(string xml)
    {
        StringReader stringReader = new StringReader(xml);
        XmlReader xmlReader = XmlReader.Create(stringReader);
        xmlReader.MoveToContent();
        return xmlReader;
    }

    [Fact]
    public void GetAttributeInt_ValidValue_ReturnsCorrectInt()
    {
        int expected = 42;

        string xml = $"<element testAttr=\"{expected}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        int actual = reader.GetAttributeInt("testAttr");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeInt_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeInt_InvalidFormat_ThrowsXmlException()
    {
        string xml = "<element testAttr=\"not-a-number\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeFloat_ValidValue_ReturnsCorrectFloat()
    {
        float expected = 3.14f;

        string xml = $"<element testAttr=\"{expected}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        float actual = reader.GetAttributeFloat("testAttr");

        Assert.Equal(expected, actual, precision: 5);
    }

    [Fact]
    public void GetAttributeFloat_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeFloat_InvalidFormat_ThrowsXmlException()
    {
        string xml = "<element testAttr=\"not-a-float\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    public void GetAttributeBool_ValidValues_ReturnsCorrectBool(string value, bool expected)
    {
        string xml = $"<element testAttr=\"{value}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        bool actual = reader.GetAttributeBool("testAttr");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeBool_InvalidFormat_ThrowsXmlException()
    {
        string xml = "<element testAttr=\"maybe\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Theory]
    [InlineData(nameof(PlayerIndex.One), PlayerIndex.One)]
    [InlineData(nameof(PlayerIndex.Two), PlayerIndex.Two)]
    [InlineData(nameof(PlayerIndex.Three), PlayerIndex.Three)]
    [InlineData(nameof(PlayerIndex.Four), PlayerIndex.Four)]
    public void GetAttributeEnum_ValidValues_ReturnsCorrectEnum(string value, PlayerIndex expected)
    {
        string xml = $"<element testAttr=\"{value}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        PlayerIndex actual = reader.GetAttributeEnum<PlayerIndex>("testAttr");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeEnum_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeEnum_InvalidValue_ThrowsXmlException()
    {
        string xml = "<element testAttr=\"InvalidEnumValue\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeRectangle_ValidValue_ReturnsCorrectRectangle()
    {
        Rectangle expected = new Rectangle(1, 2, 3, 4);

        string xml = $"<element testAttr=\"{expected.X},{expected.Y},{expected.Width},{expected.Height}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Rectangle actual = reader.GetAttributeRectangle("testAttr");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeRectangle_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Theory]
    [InlineData("1,2,3")]
    [InlineData("1,2,3,4,5")]
    [InlineData("a,b,c,d")]
    [InlineData("1.5,2,3,4")]
    public void GetAttributeRectangle_InvalidFormat_ThrowsXmlException(string value)
    {
        string xml = $"<element testAttr=\"{value}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeVector2_ValidValue_ReturnsCorrectVector2()
    {
        Vector2 expected = new Vector2(1.0f, 2.0f);

        string xml = $"<element testAttr=\"{expected.X},{expected.Y}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Vector2 actual = reader.GetAttributeVector2("testAttr");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeVector2_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Theory]
    [InlineData("3.14")]
    [InlineData("1,2,3")]
    [InlineData("a,b")]
    public void GetAttributeVector2_InvalidFormat_ThrowsXmlException(string value)
    {
        string xml = $"<element testAttr=\"{value}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Fact]
    public void GetAttributeVector3_ValidValue_ReturnsCorrectVector3()
    {
        Vector3 expected = new Vector3(1.0f, 2.0f, 3.0f);

        string xml = $"<element testAttr=\"{expected.X},{expected.Y},{expected.Z}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Vector3 actual = reader.GetAttributeVector3("testAttr");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttributeVector3_MissingAttribute_ThrowsXmlException()
    {
        string xml = "<element />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }

    [Theory]
    [InlineData("1.0,2.0")]
    [InlineData("1,2,3,4")]
    [InlineData("a,b,c")]
    public void GetAttributeVector3_InvalidFormat_ThrowsXmlException(string value)
    {
        string xml = $"<element testAttr=\"{value}\" />";
        using XmlReader reader = CreateXmlReader(xml);
        Assert.Throws<XmlException>(() => reader.GetAttributeInt("testAttr"));
    }
}
