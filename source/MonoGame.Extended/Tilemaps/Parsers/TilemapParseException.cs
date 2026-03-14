using System;

namespace MonoGame.Extended.Tilemaps.Parsers;

/// <summary>
/// Exception thrown when a tilemap file cannot be parsed.
/// </summary>
public class TilemapParseException : Exception
{
    /// <summary>
    /// Gets the file path where the error occurred, if available.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the line number where the error occurred, if available.
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// Gets the column number where the error occurred, if available.
    /// </summary>
    public int? ColumnNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public TilemapParseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public TilemapParseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="filePath">The file path where the error occurred.</param>
    /// <param name="lineNumber">The line number where the error occurred.</param>
    /// <param name="columnNumber">The column number where the error occurred.</param>
    public TilemapParseException(string message, string? filePath, int? lineNumber = null, int? columnNumber = null)
        : base(FormatMessage(message, filePath, lineNumber, columnNumber))
    {
        FilePath = filePath;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    private static string FormatMessage(string message, string filePath, int? lineNumber, int? columnNumber)
    {
        if (filePath == null)
        {
            return message;
        }

        if (!lineNumber.HasValue)
        {
            return $"{filePath}: {message}";
        }

        if (columnNumber.HasValue)
        {
            return $"{filePath}({lineNumber.Value},{columnNumber.Value}): {message}";
        }

        return $"{filePath}({lineNumber.Value}): {message}";
    }
}
