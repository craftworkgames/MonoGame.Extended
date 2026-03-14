using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public class TilemapPropertiesTests
{
    #region Indexer and Basic Operations

    [Fact]
    public void OverwritingProperty_UpdtesValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateInt(10);
        properties["key"] = TilemapPropertyValue.CreateInt(20);

        int result = properties.GetInt("key");

        Assert.Equal(20, result);
    }

    [Fact]
    public void OverwritingProperty_CanChangeType()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateInt(42);
        properties["key"] = TilemapPropertyValue.CreateString("changed");

        string result = properties.GetString("key");
        Assert.Equal("changed", result);
    }

    [Fact]
    public void TryGetValue_WithExistingKey_ReturnsTrue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateString("value");

        bool result = properties.TryGetValue("key", out TilemapPropertyValue value);

        Assert.True(result);
        Assert.Equal(TilemapPropertyType.String, value.Type);
        Assert.Equal("value", value.AsString());
    }

    [Fact]
    public void TryGetValue_WithNonExistingKey_ReturnsFalse()
    {
        TilemapProperties properties = new TilemapProperties();
        bool result = properties.TryGetValue("key", out _);
        Assert.False(result);
    }

    #endregion

    #region GetString Tests

    [Fact]
    public void GetString_WithNonExistingKey_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        string result = properties.GetString("key");
        Assert.Empty(result);
    }

    [Fact]
    public void GetString_WithWrongType_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateInt(42);

        string result = properties.GetString("key", "default");

        Assert.Equal("default", result);
    }

    #endregion

    #region GetInt Tests

    [Fact]
    public void GetInt_WithNonExistingKey_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        int result = properties.GetInt("key");
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetInt_WithCustomDefault_ReturnsCustomDefault()
    {
        TilemapProperties properties = new TilemapProperties();
        int result = properties.GetInt("key", 42);
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetInt_WithWrongType_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateString("test");

        int result = properties.GetInt("key", 42);
        Assert.Equal(42, result);
    }

    #endregion

    #region GetFloat Tests

    [Fact]
    public void GetFloat_WithNonExistingKey_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        float result = properties.GetFloat("key");
        Assert.Equal(0.0f, result);
    }

    [Fact]
    public void GetFloat_WithCustomDefault_ReturnsCustomDefault()
    {
        TilemapProperties properties = new TilemapProperties();
        float result = properties.GetFloat("key", 3.14f);
        Assert.Equal(3.14f, result);
    }

    [Fact]
    public void GetFloat_WithWrongType_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateString("test");

        float result = properties.GetFloat("key", 3.14f);
        Assert.Equal(3.14f, result);
    }

    #endregion

    #region GetBool Tests

    [Fact]
    public void GetBool_WithNonExistingKey_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        bool result = properties.GetBool("key");
        Assert.False(result);
    }

    [Fact]
    public void GetBool_WithCustomDefault_ReturnsCustomDefault()
    {
        TilemapProperties properties = new TilemapProperties();
        bool result = properties.GetBool("key", true);
        Assert.True(result);
    }

    [Fact]
    public void GetBool_WithWrongType_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateString("test");

        bool result = properties.GetBool("key", true);
        Assert.True(result);
    }

    #endregion

    #region GetColor Tests

    [Fact]
    public void GetColor_WithNonExistingKey_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        Color result = properties.GetColor("key");
        Assert.Equal(Color.White, result);
    }

    [Fact]
    public void GetColor_WithCustomDefault_ReturnsCustomDefault()
    {
        TilemapProperties properties = new TilemapProperties();
        Color result = properties.GetColor("key", Color.Orange);
        Assert.Equal(Color.Orange, result);
    }

    [Fact]
    public void GetColor_WithWrongType_ReturnsDefaultValue()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key"] = TilemapPropertyValue.CreateString("test");

        Color result = properties.GetColor("key", Color.Orange);
        Assert.Equal(Color.Orange, result);
    }

    #endregion

    #region Enumeration Tests

    [Fact]
    public void GetEnumerator_CanIterateProperties()
    {
        TilemapProperties properties = new TilemapProperties();
        properties["key1"] = TilemapPropertyValue.CreateString("test");
        properties["key2"] = TilemapPropertyValue.CreateInt(42);
        properties["key3"] = TilemapPropertyValue.CreateBool(true);

        List<KeyValuePair<string, TilemapPropertyValue>> list = properties.ToList();

        Assert.Equal(3, list.Count);
        Assert.Contains(list, kvp => kvp.Key == "key1" && kvp.Value.Type == TilemapPropertyType.String);
        Assert.Contains(list, kvp => kvp.Key == "key2" && kvp.Value.Type == TilemapPropertyType.Int);
        Assert.Contains(list, kvp => kvp.Key == "key3" && kvp.Value.Type == TilemapPropertyType.Bool);
    }

    #endregion
}
