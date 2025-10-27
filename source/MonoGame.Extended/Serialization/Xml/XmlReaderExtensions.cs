// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Serialization.Xml;

/// <summary>
/// Provides extension methods for <see cref="XmlReader"/> to simplify reading and parsing XML Attributes into
/// strong-typed values.
/// </summary>
public static class XmlReaderExtensions
{
    /// <summary>
    /// Reads an XML attribute as an <see langword="int"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see langword="int"/> value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as an <see langword="int"/>.
    /// </exception>
    public static int GetAttributeInt(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        try
        {
            return int.Parse(value);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException)
        {
            throw new XmlException(
                $"Invalid integer format for attribute '{attributeName}'. Expected integer, but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as an <see langword="int"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see langword="int"/> value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static int GetAttributeInt(this XmlReader reader, string attributeName, int defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        try
        {
            return int.Parse(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see langword="float"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see langword="float"/> value parsed from the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as a <see langword="float"/>.
    /// </exception>
    public static float GetAttributeFloat(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        try
        {
            return float.Parse(value);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException)
        {
            throw new XmlException(
                $"Invalid float format for attribute '{attributeName}'. Expected float, but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see langword="float"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see langword="float"/> value parsed from the specified attribute, or the default value if parsing fails.</returns>
    public static float GetAttributeFloat(this XmlReader reader, string attributeName, float defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        try
        {
            return float.Parse(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see langword="bool"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see langword="bool"/> value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as a <see langword="bool"/>.
    /// </exception>
    public static bool GetAttributeBool(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        try
        {
            return bool.Parse(value);
        }
        catch (Exception ex) when (ex is FormatException)
        {
            throw new XmlException(
                $"Invalid bool format for attribute '{attributeName}'. Expected 'true' or 'false' but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see langword="bool"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see langword="bool"/> value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static bool GetAttributeBool(this XmlReader reader, string attributeName, bool defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        try
        {
            return bool.Parse(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as an enumeration value.
    /// </summary>
    /// <typeparam name="T">The enumeration type to parse.</typeparam>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The enumeration value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as the specified enumeration type.
    /// </exception>
    public static T GetAttributeEnum<T>(this XmlReader reader, string attributeName) where T : struct, Enum
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        try
        {
            return Enum.Parse<T>(value);
        }
        catch (Exception ex) when (ex is ArgumentException)
        {
            throw new XmlException(
                $"Invalid {typeof(T).Name} format for attribute '{attributeName}'. Expected a {typeof(T).Name} value but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as an enumeration value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <typeparam name="T">The enumeration type to parse.</typeparam>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The enumeration value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static T GetAttributeEnum<T>(this XmlReader reader, string attributeName, T defaultValue) where T : struct, Enum
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        try
        {
            return Enum.Parse<T>(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Rectangle"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see cref="Rectangle"/> value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as a <see cref="Rectangle"/>.
    /// </exception>
    public static Rectangle GetAttributeRectangle(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 4)
            {
                throw new InvalidOperationException($"{nameof(Rectangle)} attribute must contain four integer values separated by a comma");
            }

            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            int width = int.Parse(split[2]);
            int height = int.Parse(split[3]);

            return new Rectangle(x, y, width, height);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is InvalidOperationException)
        {
            throw new XmlException(
                $"Invalid {nameof(Rectangle)} format for attribute '{attributeName}'. Expected 'x,y,width,height' but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Rectangle"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see cref="Rectangle"/> value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static Rectangle GetAttributeRectangle(this XmlReader reader, string attributeName, Rectangle defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 4)
            {
                return defaultValue;
            }

            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            int width = int.Parse(split[2]);
            int height = int.Parse(split[3]);

            return new Rectangle(x, y, width, height);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Vector2"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see cref="Vector2"/> value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as a <see cref="Vector2"/>.
    /// </exception>
    public static Vector2 GetAttributeVector2(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 2)
            {
                throw new InvalidOperationException($"{nameof(Vector2)} attribute must contain two float values separated by a comma");
            }

            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);

            return new Vector2(x, y);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is InvalidOperationException)
        {
            throw new XmlException(
                $"Invalid {nameof(Vector2)} format for attribute '{attributeName}'. Expected 'x,y', but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Vector2"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see cref="Vector2"/> value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static Vector2 GetAttributeVector2(this XmlReader reader, string attributeName, Vector2 defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 2)
            {
                return defaultValue;
            }

            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);

            return new Vector2(x, y);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Vector3"/> value.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <returns>The <see cref="Vector3"/> value parsed from the value of the specified attribute.</returns>
    /// <exception cref="XmlException">
    /// Thrown when the attribute is missing, or when the attribute value cannot be parsed as a <see cref="Vector3"/>.
    /// </exception>
    public static Vector3 GetAttributeVector3(this XmlReader reader, string attributeName)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            throw new XmlException($"Required attribute '{attributeName}' is missing.");
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 3)
            {
                throw new InvalidOperationException($"{nameof(Vector3)} attribute must contain three float values separated by a comma");
            }

            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);
            float z = float.Parse(split[2]);

            return new Vector3(x, y, z);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is InvalidOperationException)
        {
            throw new XmlException(
                $"Invalid {nameof(Vector3)} format for attribute '{attributeName}'. Expected 'x,y,z', but got '{value}'",
                ex
            );
        }
    }

    /// <summary>
    /// Reads an XML attribute as a <see cref="Vector3"/> value, returning a default value if the attribute is missing or invalid.
    /// </summary>
    /// <param name="reader">The XML reader instance.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="defaultValue">The default value to return if the attribute is missing or cannot be parsed.</param>
    /// <returns>The <see cref="Vector3"/> value parsed from the value of the specified attribute, or the default value if parsing fails.</returns>
    public static Vector3 GetAttributeVector3(this XmlReader reader, string attributeName, Vector3 defaultValue)
    {
        string value = reader.GetAttribute(attributeName);

        if (value == null)
        {
            return defaultValue;
        }

        string[] split = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        try
        {
            if (split.Length != 3)
            {
                return defaultValue;
            }

            float x = float.Parse(split[0]);
            float y = float.Parse(split[1]);
            float z = float.Parse(split[2]);

            return new Vector3(x, y, z);
        }
        catch
        {
            return defaultValue;
        }
    }
}
