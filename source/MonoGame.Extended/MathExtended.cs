using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

public static class MathExtended
{
    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <returns>
    /// The calculated <see cref="Vector2"/> value with the component-wise minimum values.</returns>
    public static Vector2 CalculateMinimumVector2(Vector2 first, Vector2 second)
    {
        return new Vector2
        {
            X = first.X < second.X ? first.X : second.X,
            Y = first.Y < second.Y ? first.Y : second.Y
        };
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <param name="result">
    /// When this method returns, contains the calculated <see cref="Vector2"/> value with the component-wise minimum
    /// values. This parameter is passed uninitialized.
    /// </param>
    public static void CalculateMinimumVector2(Vector2 first, Vector2 second, out Vector2 result)
    {
        result.X = first.X < second.X ? first.X : second.X;
        result.Y = first.Y < second.Y ? first.Y : second.Y;
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <returns>The calculated <see cref="Vector2"/> value with the component-wise maximum values.</returns>
    public static Vector2 CalculateMaximumVector2(Vector2 first, Vector2 second)
    {
        return new Vector2
        {
            X = first.X > second.X ? first.X : second.X,
            Y = first.Y > second.Y ? first.Y : second.Y
        };
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise  values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <param name="result">
    /// When this method returns, contains the calculated <see cref="Vector2"/> value with the component-wise maximum
    /// values. This parameter is passed uninitialized.
    /// </param>
    public static void CalculateMaximumVector2(Vector2 first, Vector2 second, out Vector2 result)
    {
        result.X = first.X > second.X ? first.X : second.X;
        result.Y = first.Y > second.Y ? first.Y : second.Y;
    }
}
