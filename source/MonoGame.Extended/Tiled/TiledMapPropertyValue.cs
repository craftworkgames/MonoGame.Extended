using System;

namespace MonoGame.Extended.Tiled;

[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
public class TiledMapPropertyValue
{
    public string Value { get; }

    public TiledMapProperties Properties;

    public TiledMapPropertyValue()
    {
        Value = string.Empty;
        Properties = new();
    }

    public TiledMapPropertyValue(string value)
    {
        Value = value;
        Properties = new();
    }

    public TiledMapPropertyValue(TiledMapProperties properties)
    {
        Value = string.Empty;
        Properties = properties;
    }

    public override string ToString() => Value;

    //public static implicit operator TiledMapPropertyValue(string value) => new(value);
    public static implicit operator string(TiledMapPropertyValue value) => value.Value;
    public static implicit operator TiledMapProperties(TiledMapPropertyValue value) => value.Properties;
}
