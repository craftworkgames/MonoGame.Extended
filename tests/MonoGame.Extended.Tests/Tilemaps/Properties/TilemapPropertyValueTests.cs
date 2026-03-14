using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapPropertyValueTests
{
    #region AsString Tests

    [Theory]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.File)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsString_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsString());
    }

    #endregion

    #region AsInt Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.File)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsInt_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsInt());
    }

    #endregion

    #region AsFloat Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.File)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsFloat_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsFloat());
    }

    #endregion

    #region AsBool Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.File)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsBool_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsBool());
    }

    #endregion

    #region AsColor Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.File)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsColor_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsColor());
    }

    #endregion

    #region AsFile Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.Object)]
    public void AsFile_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsFile());
    }

    #endregion

    #region AsObject Tests

    [Theory]
    [InlineData(TilemapPropertyType.String)]
    [InlineData(TilemapPropertyType.Int)]
    [InlineData(TilemapPropertyType.Float)]
    [InlineData(TilemapPropertyType.Bool)]
    [InlineData(TilemapPropertyType.Color)]
    [InlineData(TilemapPropertyType.File)]
    public void AsObject_WithWrongType_ThrowsInvalidOperationException(TilemapPropertyType type)
    {
        TilemapPropertyValue value = CreateValueOfType(type);
        Assert.Throws<InvalidOperationException>(() => value.AsObject());
    }

    #endregion

    #region Helper Methods

    private static TilemapPropertyValue CreateValueOfType(TilemapPropertyType type)
    {
        return type switch
        {
            TilemapPropertyType.String => TilemapPropertyValue.CreateString("test"),
            TilemapPropertyType.Int => TilemapPropertyValue.CreateInt(42),
            TilemapPropertyType.Float => TilemapPropertyValue.CreateFloat(3.14f),
            TilemapPropertyType.Bool => TilemapPropertyValue.CreateBool(true),
            TilemapPropertyType.Color => TilemapPropertyValue.CreateColor(Color.Red),
            TilemapPropertyType.File => TilemapPropertyValue.CreateFile("image.png"),
            TilemapPropertyType.Object => TilemapPropertyValue.CreateObject(123),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    #endregion
}
