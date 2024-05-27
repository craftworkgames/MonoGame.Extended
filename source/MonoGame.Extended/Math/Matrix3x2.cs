using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

/// <summary>
/// Represents a 3x2 matrix using floating point values for each component that can store two dimensional translation,
/// scale, and rotation information for a right-handed coordinate system.
/// </summary>
/// <remarks>
///     <para>
///         Matrices use a row vector layout in the XNA / MonoGame Framework but, in general, matrices can be either
///         have a row vector or column vector layout. Row vector matrices view vectors as a row from left to right,
///         while column vector matrices view vectors as a column from top to bottom. For example, the
///         <see cref="Translation" /> corresponds to the fields <see cref="M31" /> and <see cref="M32" />.
///     </para>
///     <para>
///         The fields see <b>M13</b> and <b>M23</b> always have a value of <c>0.0f</c>, and thus are removed from
///         the <see cref="Matrix3x2" /> to reduce its memory footprint. Same is true for the field <b>M33</b>, except
///         it always has a value of <c>1.0f</c>.
///     </para>
/// </remarks>
[DebuggerDisplay($"{nameof(DebugDisplayString)},nq")]
public struct Matrix3x2 : IEquatable<Matrix3x2>
{
    /// <summary>The first element of the first row.</summary>
    /// <remarks>Represents the scaling factor on the x-axis or a combination of scaling and rotation.</remarks>
    public float M11;

    /// <summary>The second element of the first row.</summary>
    /// <remarks>Represents the shearing factor on the y-axis or a combination of shearing and rotation.</remarks>
    public float M12;

    /// <summary>The first element of the second row.</summary>
    /// <remarks>Represents the shearing factor on the x-axis or a combination of shear and rotation.</remarks>
    public float M21;

    /// <summary>The second element of the second row.</summary>
    /// <remarks>Represents the scaling factor on the y-axis or a combination of scale and rotation.</remarks>
    public float M22;

    /// <summary>The first element of the third row.</summary>
    /// <remarks>Represents the translation on the x-axis</remarks>
    public float M31;

    /// <summary>The second element of the third row.</summary>
    /// <remarks>Represents the translation on the y-axis</remarks>
    public float M32;

    /// <summary>
    /// Gets or Sets the vector formed by the first row of this <see cref="Matrix3x2"/>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="Vector2.X"/> component of the vector represents the scaling factor on the x-axis or a
    ///         combination of scaling and rotation.
    ///     </para>
    ///     <para>
    ///         The <see cref="Vector2.Y"/> component of the vector represents the shearing factor on the y-axis or a
    ///         combination of shearing and rotation.
    ///     </para>
    /// </remarks>
    public Vector2 X
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Unsafe.As<float, Vector2>(ref Unsafe.AsRef(in M11));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Unsafe.As<float, Vector2>(ref M11) = value;
    }

    /// <summary>
    /// Gets the vector formed by the second row of this <see cref="Matrix3x2"/>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="Vector2.X"/> component of the vector represents the shearing factor on the x-axis or a
    ///         combination of shearing and rotation.
    ///     </para>
    ///     <para>
    ///         The <see cref="Vector2.Y"/> component of the vector represents the scaling factor on the y-axis or a
    ///         combination of scaling and rotation.
    ///     </para>
    /// </remarks>
    public Vector2 Y
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Unsafe.As<float, Vector2>(ref Unsafe.AsRef(in M21));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Unsafe.As<float, Vector2>(ref M21) = value;
    }

    /// <summary>
    /// Gets or Sets the vector formed by the third row of this <see cref="Matrix3x2"/>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="Vector2.X"/> component of the vector represents the translation on the x-axis.
    ///     </para>
    ///     <para>
    ///         The <see cref="Vector2.Y"/> component of the vector represents the translation on the y-axis.
    ///     </para>
    /// </remarks>
    public Vector2 Z
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Unsafe.As<float, Vector2>(ref Unsafe.AsRef(in M31));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Unsafe.As<float, Vector2>(ref M31) = value;
    }

    /// <summary>
    /// Gets the multiplicative identity matrix.
    /// </summary>
    /// <remarks>
    ///     <para>The first row of the identity matrix is equal to <see cref="Vector2.UnitX"/></para>
    ///     <para>The second row of the identity matrix is equal to <see cref="Vector2.UnitY"/></para>
    ///     <para>The third row of the identity matrix is equal to <see cref="Vector2.Zero"/></para>
    /// </remarks>
    public static readonly Matrix3x2 Identity = new Matrix3x2(Vector2.UnitX, Vector2.UnitY, Vector2.Zero);

    /// <summary>
    /// Gets the translation component of this <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// The translation is equal to the third row vector <see cref="Z"/> composed of the <see cref="M31"/> and
    /// <see cref="M32"/> values.
    /// </remarks>
    [Obsolete("Use Decompose method.  This property will be removed in 4.0")]
    public readonly Vector2 Translation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Z;
    }

    /// <summary>
    /// Gets the rotation component of this <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// The rotation is equal to the arctangent of the <see cref="M21"/> and <see cref="M11"/>.
    /// <code>Math.Atan2(M21, M11);</code>
    /// </remarks>
    [Obsolete("Use Decompose method.  This property will be removed in 4.0")]
    public readonly float Rotation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (float)Math.Atan2(M21, M11);
    }

    /// <summary>
    /// Gets the scale component of this <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// The scale is equal to equal to the square root of the sum of the squares of matrix elements, with sign
    /// adjustment.
    /// </remarks>
    [Obsolete("Use Decompose method.  This property will be removed in 4.0")]
    public readonly Vector2 Scale
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var xSign = Math.Sign((M11 * M11) + (M21 * M21)) < 0 ? -1 : 1;
            var ySign = Math.Sign((M11 * M12) + (M22 * M22)) < 0 ? -1 : 1;

            Vector2 scale;
            scale.X = xSign * (float)Math.Sqrt((M11 * M11) + (M21 * M21));
            scale.Y = ySign * (float)Math.Sqrt((M11 * M12) + (M22 * M22));

            return scale;
        }
    }

    /// <summary>
    /// Creates a 3x2 matrix from the specified components.
    /// </summary>
    /// <param name="m11">The value to assign to the first element of the first row.</param>
    /// <param name="m12">The value to assign to the second element of the first row.</param>
    /// <param name="m21">The value to assign to the first element of the second row.</param>
    /// <param name="m22">The value to assign to the second element of the second row.</param>
    /// <param name="m31">The value to assign to the first element of the third row.</param>
    /// <param name="m32">The value to assign to the second element of the third row.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2(float m11, float m12,
                 float m21, float m22,
                 float m31, float m32)
    {
        M11 = m11;
        M12 = m12;
        M21 = m21;
        M22 = m22;
        M31 = m31;
        M32 = m32;
    }

    /// <summary>
    /// Creates a new 3x2 matrix from the specified components.
    /// </summary>
    /// <param name="x">The value to assign to the elements of the first row.</param>
    /// <param name="y">The value to assign to the elements of the second row.</param>
    /// <param name="z">The value to assign to the elements of the third row.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2(Vector2 x, Vector2 y, Vector2 z)
        : this(x.X, x.Y, y.X, y.Y, z.X, z.Y) { }


    /// <summary>
    /// Transforms the given vector by this <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="vector">The vector to transform.</param>
    /// <returns>The result of the transformation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Transform(Vector2 vector) => Transform(vector.X, vector.Y);

    /// <summary>
    /// Transforms the given vector by this <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="vector">The vector to transform.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the transformation.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(Vector2 vector, out Vector2 result) => Transform(vector.X, vector.Y, out result);

    /// <summary>
    /// Transforms a vector with the specified x- and y-coordinate component values by this <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="x">The x-coordinate component value of the vector to transform.</param>
    /// <param name="y">The y-coordinate component value of the vector to transform.</param>
    /// <returns>The result of the transformation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Transform(in float x, in float y)
    {
        Vector2 result;
        Transform(x, y, out result);
        return result;
    }

    /// <summary>
    /// Transforms a vector with the specified x- and y-coordinate component values by this <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="x">The x-coordinate component value of the vector to transform.</param>
    /// <param name="y">The y-coordinate component value of the vector to transform.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the transformation.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(in float x, in float y, out Vector2 result)
    {
        result.X = (x * M11) + (y * M21) + M31;
        result.Y = (x * M12) + (y * M22) + M32;
    }

    /// <summary>
    /// Transforms a vector with the specified x- and y-coordinate component values by this <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="x">The x-coordinate component value of the vector to transform.</param>
    /// <param name="y">The y-coordinate component value of the vector to transform.</param>
    /// <param name="result">When this method returns, contains the result of the transformation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(in float x, in float y, ref Vector3 result)
    {
        result.X = (x * M11) + (y * M21) + M31;
        result.Y = (x * M12) + (y * M22) + M32;
    }

    /// <summary>
    /// Calculates the determinant of this <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// The determinant is calculated by expanding this matrix with a third column whose values are (0, 0, 1).
    /// </remarks>
    /// <returns>The determinant of this <see cref="Matrix3x2"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Determinant() => (M11 * M22) - (M12 * M21);

    /// <summary>
    /// Deconstructs this <see cref="Matrix3x2"/> into its translation, rotation, and scale component representations.
    /// </summary>
    /// <param name="translation">
    /// When this method returns, contains the translation component of this <see cref="Matrix3x2"/>.  This parameter is
    /// passed uninitialized.
    /// </param>
    /// <param name="rotation">
    /// When this method returns, contains the rotation component of this <see cref="Matrix3x2"/>.  This parameter is
    /// passed uninitialized.
    /// </param>
    /// <param name="scale">
    /// When this method returns, contains the scale component of this <see cref="Matrix3x2"/>.  This parameter is
    /// passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Decompose(out Vector2 translation, out float rotation, out Vector2 scale)
    {
        translation.X = M31;
        translation.Y = M31;

        rotation = (float)Math.Atan2(M21, M11);

        var x = M11 * M11 + M21 * M21;
        var y = M12 * M12 + M22 * M22;

        var xSign = Math.Sign(x) < 0 ? -1 : 1;
        var ySign = Math.Sign(y) < 0 ? -1 : 1;

        scale.X = xSign * (float)Math.Sqrt(x);
        scale.Y = ySign * (float)Math.Sqrt(y);
    }

    /// <summary>
    /// Creates a new <see cref="Matrix3x2"/> value for 2D translation, rotation, and scale.
    /// </summary>
    /// <remarks>Rotation is performed along the z-axis.</remarks>
    /// <param name="position">The amount to translate the matrix by on the x- and y-axis.</param>
    /// <param name="rotation">The amount to rotate, in radians, the matrix along the z-axis. </param>
    /// <param name="scale">The amount to scale the matrix along the x- and y-axis.</param>
    /// <param name="origin">The origin point at which to scale and rotate the matrix around.</param>
    /// <returns>The resulting <see cref="Matrix3x2"/> value created by this method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateFrom(Vector2 position, in float rotation, Vector2? scale, Vector2? origin)
    {
        CreateFrom(position, rotation, scale, origin, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix3x2"/> value for 2D translation, rotation, and scale.
    /// </summary>
    /// <remarks>Rotation is performed along the z-axis.</remarks>
    /// <param name="position">The amount to translate the matrix by on the x- and y-axis.</param>
    /// <param name="rotation">The amount to rotate, in radians, the matrix along the z-axis. </param>
    /// <param name="scale">The amount to scale the matrix along the x- and y-axis.</param>
    /// <param name="origin">The origin point at which to scale and rotate the matrix around.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D translation rotation, and
    /// scale.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateFrom(Vector2 position, in float rotation, Vector2? scale, Vector2? origin, out Matrix3x2 result)
    {
        result = Identity;

        if (origin.HasValue)
        {
            result.Z = -origin.Value;
        }

        if (scale.HasValue)
        {
            CreateScale(scale.Value, out Matrix3x2 scaleMatrix);
            Multiply(result, scaleMatrix, out result);
        }

        if (rotation != 0.0f)
        {
            CreateRotationZ(-rotation, out Matrix3x2 rotationMatrix);
            Multiply(result, rotationMatrix, out result);
        }

        CreateTranslation(position, out Matrix3x2 translationMatrix);
        Multiply(result, translationMatrix, out result);
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D rotation.
    /// </summary>
    /// <remarks>Rotation is performed along the z-axis.</remarks>
    /// <param name="radians">The mount to rotate, in radians, the matrix along the z-axis.</param>
    /// <returns>The resulting <see cref="Matrix3x2"/> value created by this method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateRotationZ(in float radians)
    {
        CreateRotationZ(radians, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D rotation.
    /// </summary>
    /// <remarks>Rotation is performed along the z-axis.</remarks>
    /// <param name="radians">The mount to rotate, in radians, the matrix along the z-axis.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D rotation.  This parameter
    /// is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationZ(in float radians, out Matrix3x2 result)
    {
        var cos = (float)Math.Cos(radians);
        var sin = (float)Math.Sin(radians);

        result.M11 = cos;
        result.M12 = sin;

        result.M21 = -sin;
        result.M22 = cos;

        result.M31 = 0;
        result.M32 = 0;
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="scale">The amount to scale the matrix along the x- and y-axis.</param>
    /// <returns>The <see cref="Matrix3x2"/> value created by this method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(in float scale)
    {
        CreateScale(scale, scale, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="scale">The amount to scale the matrix along the x- and y-axis.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D scaling created.  This
    /// parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(in float scale, out Matrix3x2 result) => CreateScale(scale, scale, out result);

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="scale">A vector that represents the x- and y-axis scale factors.</param>
    /// <returns>The <see cref="Matrix3x2"/> value created by this method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(Vector2 scale)
    {
        CreateScale(scale.X, scale.Y, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="scale">A vector that represents the x- and y-axis scale factors.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D scaling.  This parameter
    /// is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(in Vector2 scale, out Matrix3x2 result) =>
         CreateScale(scale.X, scale.Y, out result);

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="xScale">The scale factor for the x-axis.</param>
    /// <param name="yScale">The scale factor for the y-axis.</param>
    /// <returns>The <see cref="Matrix3x2"/> value created by this method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(in float xScale, in float yScale)
    {
        CreateScale(xScale, yScale, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    ///  Creates a <see cref="Matrix3x2"/> value for 2D scaling.
    /// </summary>
    /// <param name="xScale">The scale factor for the x-axis.</param>
    /// <param name="yScale">The scale factor for the y-axis.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D scaling.  This parameter
    /// is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(in float xScale, in float yScale, out Matrix3x2 result)
    {
        result.M11 = xScale;
        result.M12 = 0;

        result.M21 = 0;
        result.M22 = yScale;

        result.M31 = 0;
        result.M32 = 0;
    }

    /// <summary>
    /// Creates a <see cref="Matrix3x2"/> value for 2D translation.
    /// </summary>
    /// <param name="vector">The translation vector</param>
    /// <returns>The resulting <see cref="Matrix3x2"/> value for 2D translation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateTranslation(Vector2 vector) => CreateTranslation(vector.X, vector.Y);

    /// <summary>
    /// Creates a <see cref="Matrix3x2"/> for 2D translation.
    /// </summary>
    /// <param name="vector">The translation vector</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value for 2D translation.  This
    /// parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation(Vector2 vector, out Matrix3x2 result) =>
        CreateTranslation(vector.X, vector.Y, out result);

    /// <summary>
    /// Creates a <see cref="Matrix3x2"/> value for 2D translation.
    /// </summary>
    /// <param name="x">The X-coordinate of the translation vector.</param>
    /// <param name="y">The Y-coordinate of the translation vector.</param>
    /// <returns>The resulting <see cref="Matrix3x2"/> value for 2D translation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateTranslation(in float x, in float y)
    {
        CreateTranslation(x, y, out Matrix3x2 result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix3x2"/> value for 2D translation.
    /// </summary>
    /// <param name="x">The X-coordinate of the translation vector.</param>
    /// <param name="y">The Y-coordinate of the translation vector.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix3x2"/> value.  This parameter is passed
    /// uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation(in float x, in float y, out Matrix3x2 result)
    {
        result.M11 = 1;
        result.M12 = 0;

        result.M21 = 0;
        result.M22 = 1;

        result.M31 = x;
        result.M32 = y;
    }

    /// <summary>
    /// Inverts the provided <see cref="Matrix3x2"/>
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix3x2"/> value to invert.</param>
    /// <returns>The result of inverting the <paramref name="matrix"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Invert(Matrix3x2 matrix)
    {
        Invert(ref matrix);
        return matrix;
    }

    /// <summary>
    /// Inverts the provided <see cref="Matrix3x2"/> value.
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix3x2"/> value to invert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invert(ref Matrix3x2 matrix)
    {
        var det = 1.0f / matrix.Determinant();

        matrix.M11 = matrix.M22 * det;
        matrix.M12 = -matrix.M12 * det;

        matrix.M21 = -matrix.M21 * det;
        matrix.M22 = matrix.M11 * det;

        matrix.M31 = (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det;
        matrix.M32 = -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det;
    }

    /// <summary>
    /// Adds the elements of two <see cref="Matrix3x2"/> values.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>The result of the addition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Add(Matrix3x2 left, Matrix3x2 right) => left + right;

    /// <summary>
    /// Adds the elements of two <see cref="Matrix3x2"/> values.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the addition.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(Matrix3x2 left, Matrix3x2 right, out Matrix3x2 result) => result = Add(left, right);

    /// <summary>
    /// Subtracts the elements of a <see cref="Matrix3x2"/> from the corresponding elements of another
    /// <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>The result of the subtraction.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Subtract(Matrix3x2 left, Matrix3x2 right) => left - right;

    /// <summary>
    /// Subtracts the elements of a <see cref="Matrix3x2"/> from the corresponding elements of another
    /// <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the subtraction.  This method is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Subtract(Matrix3x2 left, Matrix3x2 right, out Matrix3x2 result) => result = Subtract(left, right);

    /// <summary>
    /// Multiplies the elements of a <see cref="Matrix3x2"/> by the elements of another <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation performs matrix multiplication.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>The result of the multiplication.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Multiply(Matrix3x2 left, Matrix3x2 right) => left * right;

    /// <summary>
    /// Multiplies the elements of a <see cref="Matrix3x2"/> by the elements of another <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation performs matrix multiplication.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the multiplication.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(Matrix3x2 left, Matrix3x2 right, out Matrix3x2 result) => result = left * right;

    /// <summary>
    /// Multiplies the elements of a <see cref="Matrix3x2"/> by the elements of another <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation performs matrix multiplication.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the multiplication.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(ref Matrix3x2 left, ref Matrix3x2 right, out Matrix3x2 result) => result = left * right;

    /// <summary>
    /// Multiplies the elements of a <see cref="Matrix3x2"/> by a scalar value.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/>.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of the multiplication.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Multiply(Matrix3x2 matrix, in float scalar) => matrix * scalar;

    /// <summary>
    /// Multiplies the elements of a <see cref="Matrix3x2"/> by a scalar value.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/>.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the multiplication.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(Matrix3x2 matrix, in float scalar, out Matrix3x2 result) => result = Multiply(matrix, scalar);

    /// <summary>
    /// Divides the elements of a <see cref="Matrix3x2"/> by the elements of another <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>The result of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Divide(Matrix3x2 left, Matrix3x2 right) => left / right;

    /// <summary>
    /// Divides the elements of a <see cref="Matrix3x2"/> by the elements of another <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the division.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(Matrix3x2 left, Matrix3x2 right, out Matrix3x2 result) => result = Divide(left, right);

    /// <summary>
    /// Divides the elements of a <see cref="Matrix3x2"/> by a scalar value.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/>.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Divide(Matrix3x2 matrix, in float scalar) => matrix / scalar;

    /// <summary>
    /// Divides the elements of a <see cref="Matrix3x2"/> by a scalar value.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/>.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <param name="result">
    /// When this method returns, contains the result of the division.  This parameter is passed uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(Matrix3x2 matrix, in float scalar, out Matrix3x2 result) => result = Divide(matrix, scalar);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals([NotNullWhen(true)] object obj) => obj is Matrix3x2 other && Equals(other);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Matrix3x2 other) => this == other;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, 0, 1).
    /// </remarks>
    /// <returns>The resulting <see cref="Matrix"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix ToMatrix() => ToMatrix(0.0f);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, <paramref name="depth"/>, 1).
    /// </remarks>
    /// <param name="depth">
    /// The depth value to be used for the third row of the resulting <see cref="Matrix"/> value.
    /// </param>
    /// <returns>The resulting <see cref="Matrix"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix ToMatrix(in float depth)
    {
        ToMatrix(depth, out Matrix result);
        return result;
    }

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, 0, 1).
    /// </remarks>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix"/> value.  This parameter is passed
    /// uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToMatrix(out Matrix result) => ToMatrix(0.0f, out result);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, <paramref name="depth"/>, 1).
    /// </remarks>
    /// <param name="depth">
    /// The depth value to be used for the third row of the resulting <see cref="Matrix"/> value.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix"/> value.  This parameter is passed
    /// uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToMatrix(in float depth, out Matrix result) => ToMatrix(this, depth, out result);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, 0, 1).
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/> value.</param>
    /// <returns>The resulting <see cref="Matrix"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix ToMatrix(Matrix3x2 matrix) => ToMatrix(matrix, 0.0f);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, <paramref name="depth"/>, 1).
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/> value.</param>
    /// <param name="depth">
    /// The depth value to be used for the third row of the resulting <see cref="Matrix"/> value.
    /// </param>
    /// <returns>The resulting <see cref="Matrix"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix ToMatrix(Matrix3x2 matrix, in float depth)
    {
        ToMatrix(matrix, depth, out Matrix result);
        return result;
    }

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, 0, 1).
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/> value.</param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix"/> value.  This parameter is passed
    /// uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToMatrix(Matrix3x2 matrix, out Matrix result) => ToMatrix(matrix, 0.0f, out result);

    /// <summary>
    /// Converts the specified <see cref="Matrix3x2"/> value into a <see cref="Matrix"/> value.
    /// </summary>
    /// <remarks>
    /// The third row of the resulting <see cref="Matrix"/> is set to (0, 0, 1, 0), and the fourth row is set to
    /// (<see cref="M31"/>, <see cref="M32"/>, <paramref name="depth"/>, 1).
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/> value.</param>
    /// <param name="depth">
    /// The depth value to be used for the third row of the resulting <see cref="Matrix"/> value.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the resulting <see cref="Matrix"/> value.  This parameter is passed
    /// uninitialized.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToMatrix(Matrix3x2 matrix, in float depth, out Matrix result)
    {
        result.M11 = matrix.M11;
        result.M12 = matrix.M12;
        result.M13 = 0;
        result.M14 = 0;

        result.M21 = matrix.M21;
        result.M22 = matrix.M22;
        result.M23 = 0;
        result.M24 = 0;

        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;

        result.M41 = matrix.M31;
        result.M42 = matrix.M32;
        result.M43 = depth;
        result.M44 = 1;
    }

    /// <summary>
    /// Converts a <see cref="Matrix3x2"/> value to a <see cref="Matrix"/> value.
    /// </summary>
    /// <param name="matrix">The source <see cref="Matrix3x2"/> value.</param>
    /// <returns>The resulting <see cref="Matrix"/> value.</returns>
    public static implicit operator Matrix(Matrix3x2 matrix) => ToMatrix(matrix, 0.0f);

    /// <summary>
    /// Checks if two <see cref="Matrix3x2"/> values are equal.
    /// </summary>
    /// <remarks>
    /// Two <see cref="Matrix3x2"/> values are considered equal if all their corresponding elements are equal.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>True if the values are equal; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Matrix3x2 left, Matrix3x2 right) =>
        (left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z);

    /// <summary>
    /// Checks if two <see cref="Matrix3x2"/> values are not equal.
    /// </summary>
    /// <remarks>
    /// Two <see cref="Matrix3x2"/> values are considered not equal if any of their corresponding elements differ.
    /// </remarks>
    /// <param name="left">The first <see cref="Matrix3x2"/> value.</param>
    /// <param name="right">The second <see cref="Matrix3x2"/> value.</param>
    /// <returns>True if the values are not equal; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Matrix3x2 left, Matrix3x2 right) =>
        (left.X != right.X) || (left.Y != right.Y) || (left.Z != right.Z);

    /// <inheritdoc cref="Add(Matrix3x2, Matrix3x2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 operator +(Matrix3x2 left, Matrix3x2 right)
    {
        Matrix3x2 result;

        result.M11 = left.M11 + right.M11;
        result.M12 = left.M12 + right.M12;

        result.M21 = left.M21 + right.M21;
        result.M22 = left.M22 + right.M22;

        result.M31 = left.M31 + right.M31;
        result.M32 = left.M32 + right.M32;

        return result;
    }

    /// <inheritdoc cref="Subtract(Matrix3x2, Matrix3x2)"/>
    public static Matrix3x2 operator -(Matrix3x2 left, Matrix3x2 right)
    {
        Matrix3x2 result;

        result.M11 = left.M11 - right.M11;
        result.M12 = left.M12 - right.M12;

        result.M21 = left.M21 - right.M21;
        result.M22 = left.M22 - right.M22;

        result.M31 = left.M31 - right.M31;
        result.M32 = left.M32 - right.M32;

        return result;
    }

    /// <summary>
    /// Negates the elements of a <see cref="Matrix3x2"/>.
    /// </summary>
    /// <remarks>
    /// This operation is performed component-wise.
    /// </remarks>
    /// <param name="matrix">The source <see cref="Matrix3x2"/>.</param>
    /// <returns>The result of the negation.</returns>
    public static Matrix3x2 operator -(Matrix3x2 matrix)
    {
        Matrix3x2 result;

        result.M11 = -matrix.M11;
        result.M12 = -matrix.M12;

        result.M21 = -matrix.M21;
        result.M22 = -matrix.M22;

        result.M31 = -matrix.M31;
        result.M32 = -matrix.M32;

        return result;
    }

    /// <inheritdoc cref="Multiply(Matrix3x2, Matrix3x2)"/>
    public static Matrix3x2 operator *(Matrix3x2 left, Matrix3x2 right)
    {
        Matrix3x2 result;

        result.M11 = left.M11 * right.M11 + left.M12 * right.M21;
        result.M12 = left.M11 * right.M12 + left.M12 * right.M22;

        result.M21 = left.M21 * right.M11 + left.M22 * right.M21;
        result.M22 = left.M21 * right.M12 + left.M22 * right.M22;

        result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + right.M31;
        result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + right.M32;

        return result;
    }

    /// <inheritdoc cref="Multiply(Matrix3x2, in float)"/>
    public static Matrix3x2 operator *(Matrix3x2 matrix, in float scalar)
    {
        Matrix3x2 result;

        result.M11 = matrix.M11 * scalar;
        result.M12 = matrix.M12 * scalar;

        result.M21 = matrix.M21 * scalar;
        result.M22 = matrix.M22 * scalar;

        result.M31 = matrix.M31 * scalar;
        result.M32 = matrix.M32 * scalar;

        return result;
    }

    /// <inheritdoc cref="Divide(Matrix3x2, Matrix3x2)"/>
    public static Matrix3x2 operator /(Matrix3x2 left, Matrix3x2 right)
    {
        Matrix3x2 result;

        result.M11 = left.M11 / right.M11;
        result.M12 = left.M12 / right.M12;

        result.M21 = left.M21 / right.M21;
        result.M22 = left.M22 / right.M22;

        result.M31 = left.M31 / right.M31;
        result.M32 = left.M32 / right.M32;

        return result;
    }

    /// <inheritdoc cref="Divide(Matrix3x2, in float)"/>
    public static Matrix3x2 operator /(Matrix3x2 matrix, in float scalar)
    {
        var num = 1.0f / scalar;
        Matrix3x2 result;

        result.M11 = matrix.M11 * num;
        result.M12 = matrix.M12 * num;

        result.M21 = matrix.M21 * num;
        result.M22 = matrix.M22 * num;

        result.M31 = matrix.M31 * num;
        result.M32 = matrix.M32 * num;

        return result;
    }

    /// <summary>
    /// Returns a string representation of this <see cref="Matrix3x2"/>
    /// </summary>
    /// <returns>The string representation of this <see cref="Matrix3x2"/></returns>
    public override string ToString() =>
        $"{{M11:{M11} M12:{M12}}} {{M21:{M21} M22:{M22}}} {{M31:{M31} M32:{M32}}}";

    internal string DebugDisplayString()
    {
        if (this == Identity)
        {
            return nameof(Identity);
        }

        Decompose(out Vector2 translation, out float rotation, out Vector2 scale);
        return $"T:({translation.X:0.##},{translation.Y:0.##}), R:{MathHelper.ToDegrees(rotation):0.##}°, S:({scale.X:0.##},{scale.Y:0.##})";
    }
}
