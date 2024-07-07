using System.Collections.Generic;
using System.Text.Json.Serialization;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Content.TexturePacker;

public record TexturePackerFileContent([property: JsonPropertyName("frames")] List<TexturePackerFrame> Regions,
                                       [property: JsonPropertyName("meta")] TexturePackerMeta Meta);
public record TexturePackerPoint([property: JsonPropertyName("x")] double X,
                                 [property: JsonPropertyName("y")] double Y);
public record TexturePackerSize([property: JsonPropertyName("w")] int Width,
                                [property: JsonPropertyName("h")] int Height);

public record TexturePackerRectangle([property: JsonPropertyName("x")] int X,
                                     [property: JsonPropertyName("y")] int Y,
                                     [property: JsonPropertyName("w")] int Width,
                                     [property: JsonPropertyName("h")] int Height);

public record TexturePackerFrame([property: JsonPropertyName("filename")] string FileName,
                                 [property: JsonPropertyName("frame")] TexturePackerRectangle Frame,
                                 [property: JsonPropertyName("rotated")] bool Rotated,
                                 [property: JsonPropertyName("trimmed")] bool Trimmed,
                                 [property: JsonPropertyName("spriteSourceSize")] TexturePackerRectangle SpriteSourceSize,
                                 [property: JsonPropertyName("sourceSize")] TexturePackerSize SourceSize,
                                 [property: JsonPropertyName("pivot")] TexturePackerPoint PivotPoint);

public record TexturePackerMeta([property: JsonPropertyName("app")] string App,
                                [property: JsonPropertyName("version")] string Version,
                                [property: JsonPropertyName("image")] string Image,
                                [property: JsonPropertyName("format")] string Format,
                                [property: JsonPropertyName("size")] TexturePackerSize Size,
                                [property: JsonPropertyName("scale"), JsonConverter(typeof(FloatStringConverter))] float Scale,
                                [property: JsonPropertyName("smartupdate")] string SmartUpdate);
