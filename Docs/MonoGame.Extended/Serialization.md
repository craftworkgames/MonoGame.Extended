# Serialization

MonoGame.Extended contains various serialization helpers that work with [Newtonsoft Json.NET](http://www.newtonsoft.com/json/help/html/SerializingJSON.htm).

# Json Converters

The following XNA/MonoGame types have converters:

* `Color` via **[ColorJsonConverter](#color-jsonconverter)**
* `Vector2` via **[Vector2JsonConverter](#vector2-jsonconverter)**

The following MonoGame.Extended types have converters:

* `NinePatchRegion2D` via **[NinePatchRegion2DJsonConverter](#ninepatchregion2d-jsonconverter)**
* `Size2` via **[Size2JsonConverter](#size2-jsonconverter)**
* `Range<T>` via **[RangeJsonConverter](#range-jsonconverter)**
* `TextureRegion2D` via **[TextureRegion2DJsonConverter](#textureregion2d-jsonconverter)**
* `Thickness` via **[ThicknessJsonConverter](#thickness-jsonconverter)**

### JsonConverter Example

```csharp
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization;

struct Thing {
    public Color BootStrapBlue;
    public Vector2 Position;
}
var data = @"{
    'BootStrapBlue':'#428bca00',
    'Position':'1.1 4',
}";

var thing = JsonConvert.DeserializeObject<Thing>(data,
    new ColorJsonConverter(),
    new Vector2JsonConverter()
);

thing.BootStrapBlue; // "{R:66 G:139 B:202 A:0}"
thing.Posistion; // "{X:1.1 Y:4}"
```

## Color JsonConverter

ColorJsonConverter tells Newtonsoft Json.NET now to convert string hex values into XNA `Color` objects.

Given the JSON string value
```json
"#10203040"
```
A Color object is created.
```csharp
new Color(16, 32, 48, 64);
```

The format is a `#` followed by byte hex codes for Red, Green, Blue, and Alpha channels.

## Vector2 JsonConverter

Vector2JsonConverter tells Newtonsoft Json.NET how to serialize XNA `Vector2` objects.

Given the JSON string value
```JSON
"1.2 33"
```
A new Vector2 is created.
```csharp
new Vector2(1.2f, 33f);
```

The format is `X Y` for the x and y components of a 2D vector.

## NinePatchRegion2D JsonConverter

NinePatchRegion2DJsonConverter tells Newtonsoft Json.NET how to serialize MonoGame.Extended `NinePatchRegion2D` objects.

Given the JSON object value
```json
{"TextureRegion":"Center","Padding":"1 2 3 4"}
```
A new NinePatchRegion2D is created and the TextureRegion is looked up in the TextureRegionService.
```csharp
new NinePatchRegion2D(TextureRegion2D('Center',...), 1, 2, 3, 4)
```

The format is not a string value, but a nested JSON object where `Padding` is a `Thickness` value.

## Size2 JsonConverter

Size2JsonConverter tells Newtonsoft Json.NET how to serialize MonoGame.Extended `Size2` objects.

Given the JSON string value
```json
"10 4"
```
A new Size2 object is created.  Understood, good buddy.
```csharp
new Size2(10f, 4f)
```
The format is a JSON string containing the Width, and Height values as decimal numbers.

## Range JsonConverter

`RangeJsonConverter<T>` tells Newtonsoft Json.NET how to serialize MonoGame.Extended `Range<T>` objects.

Given the JSON string value
```json
"1 9000"
```
A new Range<T> object is created.
```csharp
new Range<int>(1, 9000);
```

The format is a JSON string containing one or two values.

## TextureRegion2D JsonConverter

TextureRegion2DJsonConverter tells Newtonsoft Json.NET how to serialize MonoGame.Extended `TextureRegion2D` objects.

Given the JSON string value
```json
"Center"
```
The TextureRegionService is queried with `GetTextureRegion("Center")`

The format is a JSON string with the name of the TextureRegion in a TextureAtlas.

## Thickness JsonConverter

ThicknessJsonConverter tells Newtonsoft Json.NET how to serialize a MonoGame.Extended `Thickness` object.

Given the JSON string value
```json
"1 2 3 4"
```
A new Thickness object is created.
```
Thickness.Parse("1 2 3 4")
```

The format is a JSON string containing a value to be parsed by Thickness.

* `"1 2 3 4"` is left:1, top:2, right:3, bottom:4
* `"2 4"` is left:2, top:4, right:2, bottom:4
* `"8"` is left:8, top:8, right:8, bottom:8
* `"1,2"` is left:1, top:2, right:1, top:2

# Extensions

## ReadAsMultiDimensional

`Newtonsoft.Json.JsonReader` is extended with `T[] ReadAsMultiDimensional<T>()`

This allows you to easily create a custom JsonConverter with a list of values.

For example a list of floats
```json
"1.1 2.3 4.5 6.1"
```
Could be parsed into an array of floats using the following JsonConverter that uses `ReadAsMultiDimensional`
```csharp
public class PathJsonConverter : JsonConverter
{
    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        float[] path = reader.ReadAsMultiDimensional<float>();
        return path;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var path = (float[]) value;
        writer.WriteValue(string.Join(" ", path));
    }
}
```
