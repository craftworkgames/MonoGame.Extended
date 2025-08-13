using System.Collections.Generic;
using System.Text.Json.Serialization;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Content.TexturePacker;

public record TexturePackerFileContent([property: JsonPropertyName("frames")] List<TexturePackerFrame> Regions,
                                       [property: JsonPropertyName("textures")] List<TexturePackerTexture> Textures,
                                       [property: JsonPropertyName("meta")] TexturePackerMeta Meta);

public record TexturePackerTexture([property: JsonPropertyName("filename")] string FileName,
                                   [property: JsonPropertyName("format")] string? Format = null,
                                   [property: JsonPropertyName("size")] TexturePackerSize Size = default,
                                   [property: JsonPropertyName("scale")] string? Scale = null,
                                   [property: JsonPropertyName("premultiplied")] bool Premultiplied = false,
                                   [property: JsonPropertyName("frames")] Dictionary<string, TexturePackerTextureFrame>? Frames = null
);

public record TexturePackerTextureFrame([property: JsonPropertyName("frame")] TexturePackerRectangle Frame,
                                        [property: JsonPropertyName("rotated")] int Rotated = 0,
                                        [property: JsonPropertyName("size")] TexturePackerSize? Size = null,
                                        [property: JsonPropertyName("offset")] TexturePackerPoint? Offset = null,
                                        [property: JsonPropertyName("pivot")] TexturePackerPointF? Pivot = null,
                                        [property: JsonPropertyName("scale9")] TexturePackerRectangle? Scale9 = null
);
public record TexturePackerPoint([property: JsonPropertyName("x")] int X,
                                 [property: JsonPropertyName("y")] int Y);
public record TexturePackerPointF([property: JsonPropertyName("x")] double X,
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
                                 [property: JsonPropertyName("pivot")] TexturePackerPointF PivotPoint);

public record TexturePackerMeta([property: JsonPropertyName("app")] string App,
                                [property: JsonPropertyName("version")] string Version,
                                [property: JsonPropertyName("image")] string Image,
                                [property: JsonPropertyName("dataformat")] string DataFormat,
                                [property: JsonPropertyName("smartupdate")] string SmartUpdate);
