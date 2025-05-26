// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Serialization.Xml;

/// <summary>
/// Provides extension methods for <see cref="XmlWriter"/> to simplify writing XML attributes from strongly-typed values
/// </summary>
public static class XmlWriterExtensions
{
    /// <summary>
    /// Writes an XML attribute from an <see langword="int"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see langword="int"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeInt(this XmlWriter writer, string attributeName, int value)
    {
        writer.WriteAttributeString(attributeName, $"{value}");
    }

    /// <summary>
    /// Writes an XML attribute from a <see langword="float"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see langword="float"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeFloat(this XmlWriter writer, string attributeName, float value)
    {
        writer.WriteAttributeString(attributeName, $"{value}");
    }

    /// <summary>
    /// Writes an XML attribute from a <see langword="bool"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see langword="bool"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeBool(this XmlWriter writer, string attributeName, bool value)
    {
        writer.WriteAttributeString(attributeName, $"{value}");
    }

    /// <summary>
    /// Writes an XML attribute from a <see cref="Rectangle"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see cref="Rectangle"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeRectangle(this XmlWriter writer, string attributeName, Rectangle value)
    {
        writer.WriteAttributeString(attributeName, $"{value.X},{value.Y},{value.Width},{value.Height}");
    }

    /// <summary>
    /// Writes an XML attribute from a <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see cref="Vector2"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeVector2(this XmlWriter writer, string attributeName, Vector2 value)
    {
        writer.WriteAttributeString(attributeName, $"{value.X},{value.Y}");
    }

    /// <summary>
    /// Writes an XML attribute from a <see cref="Vector3"/> value.
    /// </summary>
    /// <param name="writer">The XML writer instance.</param>
    /// <param name="attributeName">The name of the attribute to write.</param>
    /// <param name="value">The <see cref="Vector3"/> value to write as the attribute value.</param>
    /// <exception cref="InvalidOperationException">The writer state is not valid for this operation.</exception>
    /// <exception cref="ArgumentException">The attribute name is not valid.</exception>
    public static void WriteAttributeVector3(this XmlWriter writer, string attributeName, Vector3 value)
    {
        writer.WriteAttributeString(attributeName, $"{value.X},{value.Y},{value.Z}");
    }
}
