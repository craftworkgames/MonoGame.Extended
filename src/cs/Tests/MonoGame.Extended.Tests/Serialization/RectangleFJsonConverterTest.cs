using System.IO;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Xunit;

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
    box: ""1 1 10 10""
}
";
        var serializer = new JsonSerializer();
        serializer.Converters.Add(new RectangleFJsonConverter());
        var content = serializer.Deserialize<TestContent>(new JsonTextReader(new StringReader(jsonData)));

        Assert.Equal(1, content.Box.Left);
        Assert.Equal(1, content.Box.Top);
        Assert.Equal(10, content.Box.Width);
        Assert.Equal(10, content.Box.Height);
    }
}
