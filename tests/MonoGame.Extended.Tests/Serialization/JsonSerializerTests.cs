using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Content.ContentReaders;
using MonoGame.Extended.Content.TexturePacker;
using MonoGame.Extended.Serialization.Json;
using Xunit;

namespace MonoGame.Extended.Tests.Serialization;

public sealed class JsonSerializerTests
{
    [Fact]
    public void Vector2_RoundTrip_WithContext()
    {
        var original = new Vector2(12.5f, 34.75f);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.Vector2);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.Vector2);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Color_RoundTrip_WithContext()
    {
        var original = Color.CornflowerBlue;
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.Color);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.Color);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void HslColor_RoundTrip_WithContext()
    {
        var original = HslColor.FromRgb(Color.Red);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.HslColor);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.HslColor);
        Assert.Equal(HslColor.ToRgb(original), HslColor.ToRgb(deserialized));
    }

    [Fact]
    public void RectangleF_RoundTrip_WithContext()
    {
        var original = new RectangleF(10f, 20f, 100f, 200f);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.RectangleF);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.RectangleF);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Thickness_RoundTrip_WithContext()
    {
        var original = new Thickness(1, 2, 3, 4);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.Thickness);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.Thickness);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Size_RoundTrip_WithContext()
    {
        var original = new Size(64, 128);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.Size);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.Size);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void SizeF_RoundTrip_WithContext()
    {
        var original = new SizeF(32.5f, 64.25f);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.SizeF);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.SizeF);
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Interval_Int_RoundTrip_WithContext()
    {
        var original = new Interval<int>(5, 15);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.IntervalInt32);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.IntervalInt32);
        Assert.Equal(original.Min, deserialized.Min);
        Assert.Equal(original.Max, deserialized.Max);
    }

    [Fact]
    public void Interval_Float_RoundTrip_WithContext()
    {
        var original = new Interval<float>(1.5f, 9.5f);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.IntervalSingle);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.IntervalSingle);
        Assert.Equal(original.Min, deserialized.Min);
        Assert.Equal(original.Max, deserialized.Max);
    }

    [Fact]
    public void Interval_HslColor_RoundTrip_WithContext()
    {
        var min = HslColor.FromRgb(Color.Red);
        var max = HslColor.FromRgb(Color.Blue);
        var original = new Interval<HslColor>(min, max);
        var json = JsonSerializer.Serialize(original, ExtendedJsonSerializerContext.Default.IntervalHslColor);
        var deserialized = JsonSerializer.Deserialize(json, ExtendedJsonSerializerContext.Default.IntervalHslColor);
        Assert.Equal(HslColor.ToRgb(original.Min), HslColor.ToRgb(deserialized.Min));
        Assert.Equal(HslColor.ToRgb(original.Max), HslColor.ToRgb(deserialized.Max));
    }

    [Fact]
    public void Interval_Int_RoundTrip_BareOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IntervalJsonConverter<int>());
        var original = new Interval<int>(10, 20);
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<Interval<int>>(json, options);
        Assert.Equal(original.Min, deserialized.Min);
        Assert.Equal(original.Max, deserialized.Max);
    }

    [Fact]
    public void MultiDimensional_Float_ArrayAndDelimited()
    {
        var options = new JsonSerializerOptions();

        var jsonArray = "[1.5, 2.5, 3.5]";
        var readerArray = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonArray));
        readerArray.Read();
        var arrayResult = readerArray.ReadAsMultiDimensional<float>(options);
        Assert.Equal(new[] { 1.5f, 2.5f, 3.5f }, arrayResult);

        var jsonDelimited = "\"1.5 2.5 3.5\"";
        var readerDelimited = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonDelimited));
        readerDelimited.Read();
        var delimitedResult = readerDelimited.ReadAsMultiDimensional<float>(options);
        Assert.Equal(new[] { 1.5f, 2.5f, 3.5f }, delimitedResult);

        var jsonSingle = "42.5";
        var readerSingle = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonSingle));
        readerSingle.Read();
        var singleResult = readerSingle.ReadAsMultiDimensional<float>(options);
        Assert.Equal(new[] { 42.5f }, singleResult);
    }

    [Fact]
    public void MultiDimensional_Int_ArrayAndDelimited()
    {
        var options = new JsonSerializerOptions();

        var jsonArray = "[10, 20, 30]";
        var readerArray = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonArray));
        readerArray.Read();
        var arrayResult = readerArray.ReadAsMultiDimensional<int>(options);
        Assert.Equal(new[] { 10, 20, 30 }, arrayResult);

        var jsonDelimited = "\"10 20 30\"";
        var readerDelimited = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonDelimited));
        readerDelimited.Read();
        var delimitedResult = readerDelimited.ReadAsMultiDimensional<int>(options);
        Assert.Equal(new[] { 10, 20, 30 }, delimitedResult);

        var jsonSingle = "100";
        var readerSingle = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonSingle));
        readerSingle.Read();
        var singleResult = readerSingle.ReadAsMultiDimensional<int>(options);
        Assert.Equal(new[] { 100 }, singleResult);
    }

    [Fact]
    public void MultiDimensional_WithJsonTypeInfo()
    {
        var jsonArray = "[1.0, 2.0]";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonArray));
        reader.Read();
        var result = reader.ReadAsMultiDimensional(ExtendedJsonSerializerContext.Default.Single);
        Assert.Equal(new[] { 1.0f, 2.0f }, result);
    }

    [Fact]
    public void TexturePackerFileReader_ReadStream_DeserializesCorrectly()
    {
        var sampleJson = """
        {
            "frames": [
                {
                    "filename": "antihero-idle.png",
                    "frame": { "x": 0, "y": 0, "w": 32, "h": 32 },
                    "rotated": false,
                    "trimmed": false,
                    "spriteSourceSize": { "x": 0, "y": 0, "w": 32, "h": 32 },
                    "sourceSize": { "w": 32, "h": 32 },
                    "pivot": { "x": 0.5, "y": 0.5 }
                }
            ],
            "textures": [],
            "meta": {
                "app": "TexturePacker",
                "version": "1.0",
                "image": "spritesheet.png",
                "format": "RGBA8888",
                "size": { "w": 64, "h": 64 },
                "scale": "1",
                "smartupdate": "abc"
            }
        }
        """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sampleJson));
        var content = TexturePackerFileReader.Read(stream);

        Assert.NotNull(content);
        Assert.Single(content.Regions);
        Assert.Equal("antihero-idle.png", content.Regions[0].FileName);
        Assert.Equal(32, content.Regions[0].Frame.Width);
        Assert.Equal("spritesheet.png", content.Meta.Image);
    }

#if !FNA && !KNI
    [Fact]
    public void JsonContentTypeReader_Register_WithJsonTypeInfo()
    {
        JsonContentTypeReader<Vector2>.Register(ExtendedJsonSerializerContext.Default.Vector2);
    }
#endif
}
