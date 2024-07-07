using System.Text.Json;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Tests.Serialization;

public class RectangleFJsonConverterTest
{

    public class TestContent
    {
        public RectangleF Box { get; set; }
    }

    [Fact]
    public void ConstructorTest()
    {
        var jsonData = @"
{
    ""box"": ""1 1 10 10""
}
";
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new RectangleFJsonConverter());
        var content = JsonSerializer.Deserialize<TestContent>(jsonData, options);

        Assert.Equal(1, content.Box.Left);
        Assert.Equal(1, content.Box.Top);
        Assert.Equal(10, content.Box.Width);
        Assert.Equal(10, content.Box.Height);
    }
}
