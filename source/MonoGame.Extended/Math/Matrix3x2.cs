using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

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
    /// Gets or Sets the vector formed by the first row of this <b>Matrix3x2</b>
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
    /// Gets the vector formed by the second row of this <b>Matrix3x2</b>
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
    /// Gets or Sets the vector formed by the third row of this <b>Matrix3x2</b>
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

    public static readonly Matrix3x2 Identity = new Matrix3x2(Vector2.UnitX, Vector2.UnitY, Vector2.Zero);

    [Obsolete("Use Decompose method.  This property will be removed in 4.0")]
    public readonly Vector2 Translation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Z;
    }

    [Obsolete("Use Decompose method.  This property will be removed in 4.0")]
    public readonly float Rotation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (float)Math.Atan2(M21, M11);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2(Vector2 x, Vector2 y, Vector2 z)
        : this(x.X, x.Y, y.X, y.Y, z.X, z.Y) { }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Transform(Vector2 vector) => Transform(vector.X, vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(Vector2 vector, out Vector2 result) => Transform(vector.X, vector.Y, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Transform(in float x, in float y)
    {
        Vector2 result;
        Transform(x, y, out result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(in float x, in float y, out Vector2 result)
    {
        result.X = (x * M11) + (y * M21) + M31;
        result.Y = (x * M12) + (y * M22) + M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(in float x, in float y, ref Vector3 result)
    {
        result.X = (x * M11) + (y * M21) + M31;
        result.Y = (x * M12) + (y * M22) + M32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Determinant() => (M11 * M22) - (M12 * M21);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateFrom(Vector2 position, in float rotation, Vector2? scale, Vector2? origin)
    {
        CreateFrom(position, rotation, scale, origin, out Matrix3x2 result);
        return result;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateRotationZ(in float radians)
    {
        CreateRotationZ(radians, out Matrix3x2 result);
        return result;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(in float scale)
    {
        CreateScale(scale, scale, out Matrix3x2 result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(in float scale, out Matrix3x2 result) => CreateScale(scale, scale, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(Vector2 scale)
    {
        CreateScale(scale.X, scale.Y, out Matrix3x2 result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScale(in Vector2 scale, out Matrix3x2 result) =>
         CreateScale(scale.X, scale.Y, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateScale(in float xScale, in float yScale)
    {
        CreateScale(xScale, yScale, out Matrix3x2 result);
        return result;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateTranslation(Vector2 position) => CreateTranslation(position.X, position.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation(Vector2 position, out Matrix3x2 result) =>
        CreateTranslation(position.X, position.Y, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 CreateTranslation(in float x, in float y)
    {
        CreateTranslation(x, y, out Matrix3x2 result);
        return result;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Invert(Matrix3x2 matrix)
    {
        Invert(ref matrix);
        return matrix;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Add(Matrix3x2 matrix1, Matrix3x2 matrix2) => matrix1 + matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(Matrix3x2 matrix1, Matrix3x2 matrix2, out Matrix3x2 result) => result = Add(matrix1, matrix2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Subtract(Matrix3x2 matrix1, Matrix3x2 matrix2) => matrix1 - matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Subtract(Matrix3x2 matrix1, Matrix3x2 matrix2, out Matrix3x2 result) => result = Subtract(matrix1, matrix2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Multiply(Matrix3x2 matrix1, Matrix3x2 matrix2) => matrix1 * matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(Matrix3x2 matrix1, Matrix3x2 matrix2, out Matrix3x2 result) => result = matrix1 * matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(ref Matrix3x2 matrix1, ref Matrix3x2 matrix2, out Matrix3x2 result) => result = matrix1 * matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Multiply(Matrix3x2 matrix, in float scalar) => matrix * scalar;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(Matrix3x2 matrix, in float scalar, out Matrix3x2 result) => result = Multiply(matrix, scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Divide(Matrix3x2 matrix1, Matrix3x2 matrix2) => matrix1 / matrix2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(Matrix3x2 matrix1, Matrix3x2 matrix2, out Matrix3x2 result) => result = Divide(matrix1, matrix2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2 Divide(Matrix3x2 matrix1, in float scalar) => matrix1 / scalar;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(Matrix3x2 matrix1, in float scalar, out Matrix3x2 result) => result = Divide(matrix1, scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object obj) => obj is Matrix3x2 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Matrix3x2 other) => this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix ToMatrix() => ToMatrix(0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix ToMatrix(in float depth)
    {
        ToMatrix(depth, out Matrix result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToMatrix(out Matrix result) => ToMatrix(0.0f, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToMatrix(in float depth, out Matrix result) => ToMatrix(this, depth, out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix ToMatrix(Matrix3x2 matrix) => ToMatrix(matrix, 0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix ToMatrix(Matrix3x2 matrix, in float depth)
    {
        ToMatrix(matrix, depth, out Matrix result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToMatrix(Matrix3x2 matrix, out Matrix result) => ToMatrix(matrix, 0.0f, out result);

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

    public static implicit operator Matrix(Matrix3x2 matrix) => ToMatrix(matrix, 0.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Matrix3x2 left, Matrix3x2 right) =>
        (left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Matrix3x2 left, Matrix3x2 right) =>
        (left.X != right.X) || (left.Y != right.Y) || (left.Z != right.Z);

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
    /// Returns a string representation of this <b>Matrix3x2</b>
    /// </summary>
    /// <returns>The string representation of this <b>Matrix3x2</b></returns>
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
