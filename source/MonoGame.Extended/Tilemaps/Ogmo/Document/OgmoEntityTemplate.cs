using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.Ogmo.Document;

internal sealed class OgmoEntityTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("exportID")]
    public string ExportID { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("size")]
    public OgmoVector2 Size { get; set; }

    [JsonPropertyName("origin")]
    public OgmoVector2 Origin { get; set; }

    [JsonPropertyName("originAnchored")]
    public bool OriginAnchored { get; set; }

    [JsonPropertyName("shape")]
    public OgmoShape Shape { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("tileX")]
    public bool TileX { get; set; }

    [JsonPropertyName("tileY")]
    public bool TileY { get; set; }

    [JsonPropertyName("tileSize")]
    public OgmoVector2 TileSize { get; set; }

    [JsonPropertyName("resizeableX")]
    public bool ResizeableX { get; set; }

    [JsonPropertyName("resizeableY")]
    public bool ResizeableY { get; set; }

    [JsonPropertyName("rotatable")]
    public bool Rotatable { get; set; }

    [JsonPropertyName("rotationDegrees")]
    public float RotationDegrees { get; set; }

    [JsonPropertyName("canFlipX")]
    public bool CanFlipX { get; set; }

    [JsonPropertyName("canFlipY")]
    public bool CanFlipY { get; set; }

    [JsonPropertyName("canSetColor")]
    public bool CanSetColor { get; set; }

    [JsonPropertyName("hasNodes")]
    public bool HasNodes { get; set; }

    [JsonPropertyName("nodeLimit")]
    public int NodeLimit { get; set; }

    [JsonPropertyName("nodeDisplay")]
    public int NodeDisplay { get; set; }

    [JsonPropertyName("nodeGhost")]
    public bool NodeGhost { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }

    [JsonPropertyName("values")]
    public List<OgmoValueTemplate> Values { get; set; }

    [JsonPropertyName("texture")]
    public string Texture { get; set; }

    [JsonPropertyName("textureImage")]
    public string TextureImage { get; set; }
}
