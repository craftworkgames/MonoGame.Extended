using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Collection of custom properties for tilemap elements.
/// </summary>
public class TilemapProperties : IEnumerable<KeyValuePair<string, TilemapPropertyValue>>
{
    private readonly Dictionary<string, TilemapPropertyValue> _properties = new Dictionary<string, TilemapPropertyValue>();

    /// <summary>
    /// Gets or sets the property value for the specified key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The property value.</returns>
    public TilemapPropertyValue this[string key]
    {
        get => _properties[key];
        set => _properties[key] = value;
    }

    /// <summary>
    /// Gets the number of properties in this collection.
    /// </summary>
    public int Count => _properties.Count;

    /// <summary>
    /// Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key, if found;
    /// otherwise, the default value.
    /// </param>
    /// <returns><see langword="true"/> if the property was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string key, out TilemapPropertyValue value)
    {
        return _properties.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets a string property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or has a different type.</param>
    /// <returns>The property value, or the default value if not found or type mismatch.</returns>
    public string GetString(string key, string defaultValue = "")
    {
        if (!_properties.TryGetValue(key, out TilemapPropertyValue value))
        {
            return defaultValue;
        }

        if (value.Type != TilemapPropertyType.String)
        {
            return defaultValue;
        }

        return value.AsString();
    }

    /// <summary>
    /// Gets an integer property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or has a different type.</param>
    /// <returns>The property value, or the default value if not found or type mismatch.</returns>
    public int GetInt(string key, int defaultValue = 0)
    {
        if (!_properties.TryGetValue(key, out TilemapPropertyValue value))
        {
            return defaultValue;
        }

        if (value.Type != TilemapPropertyType.Int)
        {
            return defaultValue;
        }

        return value.AsInt();
    }

    /// <summary>
    /// Gets a float property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or has a different type.</param>
    /// <returns>The property value, or the default value if not found or type mismatch.</returns>
    public float GetFloat(string key, float defaultValue = 0.0f)
    {
        if (!_properties.TryGetValue(key, out TilemapPropertyValue value))
        {
            return defaultValue;
        }

        if (value.Type != TilemapPropertyType.Float)
        {
            return defaultValue;
        }

        return value.AsFloat();
    }

    /// <summary>
    /// Gets a bool property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or has a different type.</param>
    /// <returns>The property value, or the default value if not found or type mismatch.</returns>
    public bool GetBool(string key, bool defaultValue = false)
    {
        if (!_properties.TryGetValue(key, out TilemapPropertyValue value))
        {
            return defaultValue;
        }

        if (value.Type != TilemapPropertyType.Bool)
        {
            return defaultValue;
        }

        return value.AsBool();
    }

    /// <summary>
    /// Gets a color property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or has a different type.</param>
    /// <returns>The property value, or the default value if not found or type mismatch.</returns>
    public Color GetColor(string key, Color? defaultValue = null)
    {
        Color effectiveDefault = defaultValue ?? Color.White;

        if (!_properties.TryGetValue(key, out TilemapPropertyValue value))
        {
            return effectiveDefault;
        }

        if (value.Type != TilemapPropertyType.Color)
        {
            return effectiveDefault;
        }

        return value.AsColor();
    }

    /// <summary>
    /// Sets a string property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    public void SetString(string key, string value)
    {
        _properties[key] = TilemapPropertyValue.CreateString(value);
    }

    /// <summary>
    /// Sets an integer property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    public void SetInt(string key, int value)
    {
        _properties[key] = TilemapPropertyValue.CreateInt(value);
    }

    /// <summary>
    /// Sets a float property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    public void SetFloat(string key, float value)
    {
        _properties[key] = TilemapPropertyValue.CreateFloat(value);
    }

    /// <summary>
    /// Sets a bool property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    public void SetBool(string key, bool value)
    {
        _properties[key] = TilemapPropertyValue.CreateBool(value);
    }

    /// <summary>
    /// Sets a color property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    public void SetColor(string key, Color value)
    {
        _properties[key] = TilemapPropertyValue.CreateColor(value);
    }

    public IEnumerator<KeyValuePair<string, TilemapPropertyValue>> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
