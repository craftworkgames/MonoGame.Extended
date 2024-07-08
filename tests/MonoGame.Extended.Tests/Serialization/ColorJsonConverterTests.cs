using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Tests.Serialization;

public sealed class ColorJsonConverterTests
{
    private readonly ColorJsonConverter _converter = new ColorJsonConverter();

    [Fact]
    public void CanConvert_ColorType_ReturnsTrue()
    {
        var colorType = typeof(Color);
        var result = _converter.CanConvert(colorType);
        Assert.True(result);
    }

    [Fact]
    public void CanConvert_NonColorType_ReturnsFalse()
    {
        var otherType = typeof(string);
        var result = _converter.CanConvert(otherType);
        Assert.False(result);
    }

    [Theory]
    [InlineData("Red", 255, 0, 0, 255)]
    [InlineData("#FF0000FF", 255, 0, 0, 255)]
    public void Read_ValidColorJson_ReturnsExpectedColor(string jsonValue, byte r, byte g, byte b, byte a)
    {
        var json = $"\"{jsonValue}\"";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        reader.Read();
        var actual = _converter.Read(ref reader, typeof(Color), new JsonSerializerOptions());

        var expected = new Color(r, g, b, a);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Write_ValidColor_WritesExpectedJson()
    {
        var expected = "#ff000000";
        var color = ColorHelper.FromHex(expected);
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        _converter.Write(writer, color, new JsonSerializerOptions());
        writer.Flush();
        var actual = Encoding.UTF8.GetString(stream.ToArray());

        Assert.Equal($"\"{expected}\"", actual);
    }

    [Fact]
    public void Write_NullWrier_ThrowArgumentNullException()
    {
        var color = Color.DarkOrange;
        Assert.Throws<ArgumentNullException>(() => _converter.Write(null, color, new JsonSerializerOptions()));
    }
}
