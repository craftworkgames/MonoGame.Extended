using System;
using System.Globalization;
using System.Xml;

/// <summary>
/// Provides extension methods for the <see cref="XmlNode"/> class.
/// </summary>
public static class XmlNodeExtensions
{
    /// <summary>
    /// Retrieves the value of the specified attribute from the <see cref="XmlNode"/>.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <param name="value">
    /// When this method returns, contains the value of the attribute, if found; otherwise, an empty string.
    /// </param>
    /// <returns><c>true</c> if the attribute is found and has a non-empty value; otherwise, <c>false</c>.</returns>
    private static bool GetAttributeValue(this XmlNode node, string attribute, out string value)
    {
        value = string.Empty;
        XmlAttribute attr = node.Attributes[attribute];
        if (attr is not null)
        {
            value = attr.Value;
        }
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Retrieves the string value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The string value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static string GetStringAttribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return value;
        }
        return default;
    }

    /// <summary>
    /// Retrieves the byte value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The byte value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static byte GetByteAttribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToByte(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the ushort value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The ushort value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static ushort GetUInt16Attribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the short value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The short value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static short GetInt16Attribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToInt16(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the uint value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The uint value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static uint GetUInt32Attribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the int value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The int value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static int GetInt32Attribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the float value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The float value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static float GetSingleAttribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the double value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The double value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static double GetDoubleAttribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the boolean value of the specified attribute.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <returns>
    /// The boolean value of the attribute, or the default value if the attribute is not found or is empty.
    /// </returns>
    public static bool GetBoolAttribute(this XmlNode node, string attribute)
    {
        if (node.GetAttributeValue(attribute, out string value))
        {
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }
        return default;
    }

    /// <summary>
    /// Retrieves the byte values from the specified delimited attribute and returns them in an array.
    /// </summary>
    /// <param name="node">The XML node.</param>
    /// <param name="attribute">The name of the attribute.</param>
    /// <param name="expectedCount">The expected number of byte values.</param>
    /// <returns>
    /// An array of byte values parsed from the attribute, or an array of default values if the attribute is not found
    /// or is empty.
    /// </returns>
    public static byte[] GetByteDelimitedAttribute(this XmlNode node, string attribute, int expectedCount)
    {
        byte[] result = new byte[expectedCount];

        if (node.GetAttributeValue(attribute, out string value))
        {
            string[] split = value.Split(',');
            for (int i = 0; i < expectedCount; ++i)
            {
                result[i] = Convert.ToByte(split[i], CultureInfo.InvariantCulture);
            }
        }

        return result;
    }
}
