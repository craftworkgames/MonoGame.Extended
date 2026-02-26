using System;
using System.Linq.Expressions;

namespace MonoGame.Extended.Tweening;

/// <summary>
/// Provides compiled arithmetic delegates for a value type <typeparamref name="T"/>, enabling
/// type-agnostic linear interpolation over any struct that supports addition, subtraction, and
/// scalar multiplication operators. The delegates are compiled once via expression trees and
/// cached as static members.
/// </summary>
/// <typeparam name="T">
/// The value type to compile operations for. Must define <c>+</c>, <c>-</c>, and <c>*</c>
/// operators, with <c>*</c> accepting a <see cref="float"/> scalar as the right-hand operand.
/// </typeparam>
public class LinearOperations<T>
{
    static LinearOperations()
    {
        var a = Expression.Parameter(typeof(T));
        var b = Expression.Parameter(typeof(T));
        var c = Expression.Parameter(typeof(float));
        Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(a, b), a, b).Compile();
        Subtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(a, b), a, b).Compile();
        Multiply = Expression.Lambda<Func<T, float, T>>(Expression.Multiply(a, c), a, c).Compile();
    }

    /// <summary>
    /// Gets a compiled delegate that adds two values of type <typeparamref name="T"/>.
    /// </summary>
    public static Func<T, T, T> Add { get; }

    /// <summary>
    /// Gets a compiled delegate that subtracts the second value from the first for type <typeparamref name="T"/>.
    /// </summary>
    public static Func<T, T, T> Subtract { get; }

    /// <summary>
    /// Gets a compiled delegate that multiplies a value of type <typeparamref name="T"/> by a <see cref="float"/> scalar.
    /// </summary>
    public static Func<T, float, T> Multiply { get; }
}
