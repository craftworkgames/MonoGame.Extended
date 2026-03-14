namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Specifies the type of a custom property value.
/// </summary>
public enum TilemapPropertyType
{
    /// <summary>
    /// A string value.
    /// </summary>
    String,

    /// <summary>
    /// An integer value.
    /// </summary>
    Int,

    /// <summary>
    /// A floating-point value.
    /// </summary>
    Float,

    /// <summary>
    /// A boolean value.
    /// </summary>
    Bool,

    /// <summary>
    /// A color value.
    /// </summary>
    Color,

    /// <summary>
    /// A file path value.
    /// </summary>
    File,

    /// <summary>
    /// An object reference (ID) value.
    /// </summary>
    Object
}
