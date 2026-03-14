using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a custom property value that can be of various types.
/// </summary>
public readonly struct TilemapPropertyValue
{
    [StructLayout(LayoutKind.Explicit)]
    private readonly struct ValueData
    {
        [FieldOffset(0)] public readonly int Int32;
        [FieldOffset(0)] public readonly float Single;
        [FieldOffset(0)] public readonly bool Boolean;
        [FieldOffset(0)] public readonly uint ColorPackedValue;

        public ValueData(int value) => Int32 = value;
        public ValueData(float value) => Single = value;
        public ValueData(bool value) => Boolean = value;
        public ValueData(uint value) => ColorPackedValue = value;
    }

    private readonly ValueData _valueData;
    private readonly string _stringValue;

    /// <summary>
    /// Gets the type of this property value.
    /// </summary>
    public TilemapPropertyType Type { get; }

    private TilemapPropertyValue(TilemapPropertyType type, ValueData valueData, string stringValue = null)
    {
        Type = type;
        _valueData = valueData;
        _stringValue = stringValue;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a string property value.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>A property value containing the string.</returns>
    public static TilemapPropertyValue CreateString(string value)
    {
        return new TilemapPropertyValue(TilemapPropertyType.String, default, value);
    }

    /// <summary>
    /// Creates an integer property value.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <returns>A property value containing the integer.</returns>
    public static TilemapPropertyValue CreateInt(int value)
    {
        return new TilemapPropertyValue(TilemapPropertyType.Int, new ValueData(value));
    }

    /// <summary>
    /// Creates a float property value.
    /// </summary>
    /// <param name="value">The float value.</param>
    /// <returns>A property value containing the float.</returns>
    public static TilemapPropertyValue CreateFloat(float value)
    {
        return new TilemapPropertyValue(TilemapPropertyType.Float, new ValueData(value));
    }

    /// <summary>
    /// Creates a boolean property value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>A property value containing the boolean.</returns>
    public static TilemapPropertyValue CreateBool(bool value)
    {
        return new TilemapPropertyValue(TilemapPropertyType.Bool, new ValueData(value));
    }

    /// <summary>
    /// Creates a color property value.
    /// </summary>
    /// <param name="value">The color value.</param>
    /// <returns>A property value containing the color.</returns>
    public static TilemapPropertyValue CreateColor(Color value)
    {
        return new TilemapPropertyValue(TilemapPropertyType.Color, new ValueData(value.PackedValue));
    }

    /// <summary>
    /// Creates a file path property value.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>A property value containing the file path.</returns>
    public static TilemapPropertyValue CreateFile(string path)
    {
        return new TilemapPropertyValue(TilemapPropertyType.File, default, path);
    }

    /// <summary>
    /// Creates an object reference property value.
    /// </summary>
    /// <param name="objectId">The object reference ID.</param>
    /// <returns>A property value containing the object ID.</returns>
    public static TilemapPropertyValue CreateObject(int objectId)
    {
        return new TilemapPropertyValue(TilemapPropertyType.Object, new ValueData(objectId));
    }

    #endregion

    #region Type-safe Accessors

    /// <summary>
    /// Gets the value as a string.
    /// </summary>
    /// <returns>The string value.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.String"/></exception>
    public string AsString()
    {
        if (Type != TilemapPropertyType.String)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as String");
        }

        return _stringValue;
    }

    /// <summary>
    /// Gets the value as an integer.
    /// </summary>
    /// <returns>The integer value.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.Int"/></exception>
    public int AsInt()
    {
        if (Type != TilemapPropertyType.Int)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as Int.");
        }

        return _valueData.Int32;
    }

    /// <summary>
    /// Gets the value as a float.
    /// </summary>
    /// <returns>The float value.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.Float"/></exception>
    public float AsFloat()
    {
        if (Type != TilemapPropertyType.Float)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as Float.");
        }

        return _valueData.Single;
    }

    /// <summary>
    /// Gets the value as a boolean.
    /// </summary>
    /// <returns>The bool value.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.Bool"/></exception>
    public bool AsBool()
    {
        if (Type != TilemapPropertyType.Bool)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as Bool.");
        }

        return _valueData.Boolean;
    }

    /// <summary>
    /// Gets the value as a color.
    /// </summary>
    /// <returns>The color value.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.Color"/></exception>
    public Color AsColor()
    {
        if (Type != TilemapPropertyType.Color)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as Color.");
        }

        return new Color(_valueData.ColorPackedValue);
    }

    /// <summary>
    /// Gets the value as a file path.
    /// </summary>
    /// <returns>The file path.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.File"/></exception>
    public string AsFile()
    {
        if (Type != TilemapPropertyType.File)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as File.");
        }

        return _stringValue;
    }

    /// <summary>
    /// Gets the value as an object reference ID.
    /// </summary>
    /// <returns>The object ID.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="TilemapPropertyType.Object"/></exception>
    public int AsObject()
    {
        if (Type != TilemapPropertyType.Object)
        {
            throw new InvalidOperationException($"Cannot access property of type {Type} as Object.");
        }

        return _valueData.Int32;
    }

    #endregion
}
