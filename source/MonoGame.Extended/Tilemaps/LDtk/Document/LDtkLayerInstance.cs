using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Tilemaps.LDtk.Document;

internal sealed class LDtkLayerInstance
{
    [JsonPropertyName("__identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("__cWid")]
    public int CWid { get; set; }

    [JsonPropertyName("__cHei")]
    public int CHei { get; set; }

    [JsonPropertyName("__gridSize")]
    public int GridSize { get; set; }

    [JsonPropertyName("__opacity")]
    public float Opacity { get; set; }

    [JsonPropertyName("__pxTotalOffsetX")]
    public int PxTotalOffsetX { get; set; }

    [JsonPropertyName("__pxTotalOffsetY")]
    public int PxTotalOffsetY { get; set; }

    [JsonPropertyName("__tilesetDefUid")]
    public int? TilesetDefUid { get; set; }

    [JsonPropertyName("__tilesetRelPath")]
    public string TilesetRelPath { get; set; }

    [JsonPropertyName("iid")]
    public string Iid { get; set; }

    [JsonPropertyName("levelId")]
    public int LevelId { get; set; }

    [JsonPropertyName("layerDefUid")]
    public int LayerDefUid { get; set; }

    [JsonPropertyName("pxOffsetX")]
    public int PxOffsetX { get; set; }

    [JsonPropertyName("pxOffsetY")]
    public int PxOffsetY { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("__parallaxFactorX")]
    public float ParallaxFactorX { get; set; }

    [JsonPropertyName("__parallaxFactorY")]
    public float ParallaxFactorY { get; set; }

    [JsonPropertyName("gridTiles")]
    public List<LDtkTileInstance> GridTiles { get; set; } = new List<LDtkTileInstance>();

    [JsonPropertyName("autoLayerTiles")]
    public List<LDtkTileInstance> AutoLayerTiles { get; set; } = new List<LDtkTileInstance>();

    [JsonPropertyName("entityInstances")]
    public List<LDtkEntityInstance> EntityInstances { get; set; } = new List<LDtkEntityInstance>();

    [JsonPropertyName("intGridCsv")]
    public List<int> IntGridCsv { get; set; } = new List<int>();
}
