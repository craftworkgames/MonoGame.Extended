using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace MonoGame.Extended
{
    // https://en.wikipedia.org/wiki/Matrix_(mathematics)
    // "Immersive Linear Algebra"; Jacob Ström, Kalle Åström & Tomas Akenine-Möller; 2015-2016. Chapter 6: The Matrix. http://immersivemath.com/ila/ch06_matrices/ch06.html
    // "Real-Time Collision Detection"; Christer Ericson; 2005. Chapter 3.1: A Math and Geometry Primer - Matrices. pg 23-34

    // Original code was from Matrix2D.cs in the Nez Library: https://github.com/prime31/Nez/

    //TODO: Lerp(...), Transpose(...)

    /// <summary>
    ///     Defines a 3x3 matrix using floating point numbers which can store two dimensional translation, scale and rotation
    ///     information in a right-handed coordinate system.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Matrices use a row vector layout in the XNA / MonoGame Framework but, in general, matrices can be either have
    ///         a row vector or column vector layout. Row vector matrices view vectors as a row from left to right, while
    ///         column vector matrices view vectors as a column from top to bottom. For example, the <see cref="Translation" />
    ///         corresponds to the fields <see cref="M31" /> and <see cref="M32" />.
    ///     </para>
    ///     <para>
    ///         The fields M13 and M23 always have a value of <code>0.0f</code>, and thus are removed from the
    ///         <see cref="Matrix2D" /> to reduce its memory footprint. Same is true for the field M33, except it always has a
    ///         value of <code>1.0f</code>.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Matrix2D : IEquatable<Matrix2D>, IEquatableByRef<Matrix2D>
    {
        public float M11; // x scale, also used for rotation
        public float M12; // used for rotation

        public float M21; // used for rotation
        public float M22; // y scale, also used for rotation

        public float M31; // x translation
        public float M32; // y translation

        /// <summary>
        ///     Gets the identity matrix.
        /// </summary>
        /// <value>
        ///     The identity matrix.
        /// </value>
        public static Matrix2D Identity { get; } = new Matrix2D(1f, 0f, 0f, 1f, 0f, 0f);

        /// <summary>
        ///     Gets the translation.
        /// </summary>
        /// <value>
        ///     The translation.
        /// </value>
        /// <remarks>The <see cref="Translation" /> is equal to the vector <code>(M31, M32)</code>.</remarks>
        public Vector2 Translation => new Vector2(M31, M32);

        /// <summary>
        ///     Gets the rotation angle in radians.
        /// </summary>
        /// <value>
        ///     The rotation angle in radians.
        /// </value>
        /// <remarks>
        ///     The <see cref="Rotation" /> is equal to <code>Atan2(M21, M11)</code>.
        /// </remarks>
        public float Rotation => (float)Math.Atan2(M21, M11);

        /// <summary>
        ///     Gets the scale.
        /// </summary>
        /// <value>
        ///     The scale.
        /// </value>
        /// <remarks>
        ///     The <see cref="Scale" /> is equal to the vector
        ///     <code>(Sqrt(M11 * M11 + M21 * M21), Sqrt(M12 * M12 + M22 * M22))</code>.
        /// </remarks>
        public Vector2 Scale
        {
            get
            {
                var scaleX = (float)Math.Sqrt(M11 * M11 + M21 * M21);
                var scaleY = (float)Math.Sqrt(M12 * M12 + M22 * M22);
                return new Vector2(scaleX, scaleY);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct.
        /// </summary>
        /// <param name="m11">The value to initialize <see cref="M11" /> to.</param>
        /// <param name="m12">The value to initialize <see cref="M12" /> to.</param>
        /// <param name="m21">The value to initialize <see cref="M21" /> to.</param>
        /// <param name="m22">The value to initialize <see cref="M22" /> to.</param>
        /// <param name="m31">The value to initialize <see cref="M31" /> to.</param>
        /// <param name="m32">The value to initialize <see cref="M32" /> to.</param>
        /// <remarks>
        ///     <para>
        ///         The fields M13 and M23 always have a value of <code>0.0f</code>, and thus are removed from the
        ///         <see cref="Matrix2D" /> to reduce its memory footprint. Same is true for the field M33, except it always has a
        ///         value of <code>1.0f</code>.
        ///     </para>
        /// </remarks>
        public Matrix2D(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11;
            M12 = m12;

            M21 = m21;
            M22 = m22;

            M31 = m31;
            M32 = m32;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> by this <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <returns>The resulting <see cref="Vector2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Transform(Vector2 vector)
        {
            Vector2 result;
            Transform(vector, out result);
            return result;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> by this <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <param name="result">The resulting <see cref="Vector2" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Transform(Vector2 vector, out Vector2 result)
        {
            result.X = vector.X * M11 + vector.Y * M21 + M31;
            result.Y = vector.X * M12 + vector.Y * M22 + M32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to translate, rotate, and scale a set of vertices in two dimensions.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="rotation">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="origin">The point which to rotate and scale around.</param>
        /// <param name="transformMatrix">The resulting <see cref="Matrix2D" /></param>
        public static void CreateFrom(Vector2 position, float rotation, Vector2? scale, Vector2? origin,
            out Matrix2D transformMatrix)
        {
            transformMatrix = Identity;

            if (origin.HasValue)
            {
                transformMatrix.M31 = -origin.Value.X;
                transformMatrix.M32 = -origin.Value.Y;
            }

            if (scale.HasValue)
            {
                var scaleMatrix = CreateScale(scale.Value);
                Multiply(ref transformMatrix, ref scaleMatrix, out transformMatrix);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation != 0f)
            {
                var rotationMatrix = CreateRotationZ(-rotation);
                Multiply(ref transformMatrix, ref rotationMatrix, out transformMatrix);
            }

            var translationMatrix = CreateTranslation(position);
            Multiply(ref transformMatrix, ref translationMatrix, out transformMatrix);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to translate, rotate, and scale a set of vertices in two dimensions.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="rotation">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="origin">The point which to rotate and scale around.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateFrom(Vector2 position, float rotation, Vector2? scale = null, Vector2? origin = null)
        {
            var transformMatrix = Identity;

            if (origin.HasValue)
            {
                transformMatrix.M31 = -origin.Value.X;
                transformMatrix.M32 = -origin.Value.Y;
            }

            if (scale.HasValue)
            {
                var scaleMatrix = CreateScale(scale.Value);
                transformMatrix = Multiply(transformMatrix, scaleMatrix);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation != 0f)
            {
                var rotationMatrix = CreateRotationZ(-rotation);
                transformMatrix = Multiply(transformMatrix, rotationMatrix);
            }

            var translationMatrix = CreateTranslation(position);
            transformMatrix = Multiply(transformMatrix, translationMatrix);

            return transformMatrix;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to rotate a set of vertices
        ///     around the z-axis.
        /// </summary>
        /// <param name="radians">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateRotationZ(float radians)
        {
            Matrix2D result;
            CreateRotationZ(radians, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to rotate a set of vertices around the z-axis.
        /// </summary>
        /// <param name="radians">The amount, in radians, in which to rotate around the z-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateRotationZ(float radians, out Matrix2D result)
        {
            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);

            result.M11 = val1;
            result.M12 = val2;

            result.M21 = -val2;
            result.M22 = val1;

            result.M31 = 0;
            result.M32 = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateScale(float scale)
        {
            Matrix2D result;
            CreateScale(scale, scale, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amount to scale by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateScale(float scale, out Matrix2D result)
        {
            CreateScale(scale, scale, out result);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="xScale">The amount to scale by on the x-axis.</param>
        /// <param name="yScale">The amount to scale by on the y-axis.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateScale(float xScale, float yScale)
        {
            Matrix2D result;
            CreateScale(xScale, yScale, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="xScale">The amount to scale by on the x-axis.</param>
        /// <param name="yScale">The amount to scale by on the y-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateScale(float xScale, float yScale, out Matrix2D result)
        {
            result.M11 = xScale;
            result.M12 = 0;

            result.M21 = 0;
            result.M22 = yScale;

            result.M31 = 0;
            result.M32 = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amounts to scale by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateScale(Vector2 scale)
        {
            Matrix2D result;
            CreateScale(ref scale, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to scale a set vertices.
        /// </summary>
        /// <param name="scale">The amounts to scale by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateScale(ref Vector2 scale, out Matrix2D result)
        {
            result.M11 = scale.X;
            result.M12 = 0;

            result.M21 = 0;
            result.M22 = scale.Y;

            result.M31 = 0;
            result.M32 = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="xPosition">The amount to translate by on the x-axis.</param>
        /// <param name="yPosition">The amount to translate by on the y-axis.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateTranslation(float xPosition, float yPosition)
        {
            Matrix2D result;
            CreateTranslation(xPosition, yPosition, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="xPosition">The amount to translate by on the x-axis.</param>
        /// <param name="yPosition">The amount to translate by on the y-axis.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateTranslation(float xPosition, float yPosition, out Matrix2D result)
        {
            result.M11 = 1;
            result.M12 = 0;

            result.M21 = 0;
            result.M22 = 1;

            result.M31 = xPosition;
            result.M32 = yPosition;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D CreateTranslation(Vector2 position)
        {
            Matrix2D result;
            CreateTranslation(ref position, out result);
            return result;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> struct that can be used to translate a set vertices.
        /// </summary>
        /// <param name="position">The amounts to translate by on the x and y axes.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void CreateTranslation(ref Vector2 position, out Matrix2D result)
        {
            result.M11 = 1;
            result.M12 = 0;

            result.M21 = 0;
            result.M22 = 1;

            result.M31 = position.X;
            result.M32 = position.Y;
        }

        /// <summary>
        ///     Calculates the determinant of the <see cref="Matrix2D" />.
        /// </summary>
        /// <returns>The determinant of the <see cref="Matrix2D" />.</returns>
        public float Determinant()
        {
            return M11 * M22 - M12 * M21;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the summation of two <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Add(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;

            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;

            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;

            return matrix1;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> summation of two <see cref="Matrix2D" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Add(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;

            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;

            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the summation of two <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator +(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 = matrix1.M11 + matrix2.M11;
            matrix1.M12 = matrix1.M12 + matrix2.M12;

            matrix1.M21 = matrix1.M21 + matrix2.M21;
            matrix1.M22 = matrix1.M22 + matrix2.M22;

            matrix1.M31 = matrix1.M31 + matrix2.M31;
            matrix1.M32 = matrix1.M32 + matrix2.M32;

            return matrix1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the substraction of two
        ///     <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Subtract(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 = matrix1.M11 - matrix2.M11;
            matrix1.M12 = matrix1.M12 - matrix2.M12;

            matrix1.M21 = matrix1.M21 - matrix2.M21;
            matrix1.M22 = matrix1.M22 - matrix2.M22;

            matrix1.M31 = matrix1.M31 - matrix2.M31;
            matrix1.M32 = matrix1.M32 - matrix2.M32;
            return matrix1;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> substraction of two <see cref="Matrix2D" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Subtract(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
        {
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;

            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;

            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the substraction of two
        ///     <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator -(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 = matrix1.M11 - matrix2.M11;
            matrix1.M12 = matrix1.M12 - matrix2.M12;

            matrix1.M21 = matrix1.M21 - matrix2.M21;
            matrix1.M22 = matrix1.M22 - matrix2.M22;

            matrix1.M31 = matrix1.M31 - matrix2.M31;
            matrix1.M32 = matrix1.M32 - matrix2.M32;
            return matrix1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the multiplication of two
        ///     <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Multiply(Matrix2D matrix1, Matrix2D matrix2)
        {
            var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
            var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;

            var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
            var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;

            var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix2.M31;
            var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix2.M32;

            matrix1.M11 = m11;
            matrix1.M12 = m12;

            matrix1.M21 = m21;
            matrix1.M22 = m22;

            matrix1.M31 = m31;
            matrix1.M32 = m32;

            return matrix1;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> multiplication of two <see cref="Matrix2D" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Multiply(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
        {
            var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
            var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;

            var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
            var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;

            var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix2.M31;
            var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix2.M32;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of a <see cref="Matrix2D" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2D" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Multiply(Matrix2D matrix, float scalar)
        {
            matrix.M11 *= scalar;
            matrix.M12 *= scalar;

            matrix.M21 *= scalar;
            matrix.M22 *= scalar;

            matrix.M31 *= scalar;
            matrix.M32 *= scalar;
            return matrix;
        }

        /// <summary>
        ///     Calculates the multiplication of a <see cref="Matrix2D" /> by a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to multiple the <see cref="Matrix2D" /> by.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Multiply(ref Matrix2D matrix, float scalar, out Matrix2D result)
        {
            result.M11 = matrix.M11 * scalar;
            result.M12 = matrix.M12 * scalar;

            result.M21 = matrix.M21 * scalar;
            result.M22 = matrix.M22 * scalar;

            result.M31 = matrix.M31 * scalar;
            result.M32 = matrix.M32 * scalar;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the multiplication of two
        ///     <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator *(Matrix2D matrix1, Matrix2D matrix2)
        {
            var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
            var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;

            var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
            var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;

            var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix2.M31;
            var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix2.M32;

            matrix1.M11 = m11;
            matrix1.M12 = m12;

            matrix1.M21 = m21;
            matrix1.M22 = m22;

            matrix1.M31 = m31;
            matrix1.M32 = m32;

            return matrix1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of a <see cref="Matrix2D" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2D" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator *(Matrix2D matrix, float scalar)
        {
            matrix.M11 = matrix.M11 * scalar;
            matrix.M12 = matrix.M12 * scalar;

            matrix.M21 = matrix.M21 * scalar;
            matrix.M22 = matrix.M22 * scalar;

            matrix.M31 = matrix.M31 * scalar;
            matrix.M32 = matrix.M32 * scalar;

            return matrix;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of two <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Divide(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 = matrix1.M11 / matrix2.M11;
            matrix1.M12 = matrix1.M12 / matrix2.M12;

            matrix1.M21 = matrix1.M21 / matrix2.M21;
            matrix1.M22 = matrix1.M22 / matrix2.M22;

            matrix1.M31 = matrix1.M31 / matrix2.M31;
            matrix1.M32 = matrix1.M32 / matrix2.M32;
            return matrix1;
        }

        /// <summary>
        ///     Calculates the <see cref="Matrix2D" /> division of two <see cref="Matrix2D" />s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Divide(ref Matrix2D matrix1, ref Matrix2D matrix2, out Matrix2D result)
        {
            result.M11 = matrix1.M11 / matrix2.M11;
            result.M12 = matrix1.M12 / matrix2.M12;

            result.M21 = matrix1.M21 / matrix2.M21;
            result.M22 = matrix1.M22 / matrix2.M22;

            result.M31 = matrix1.M31 / matrix2.M31;
            result.M32 = matrix1.M32 / matrix2.M32;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of a <see cref="Matrix2D" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2D" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Divide(Matrix2D matrix, float scalar)
        {
            var num = 1f / scalar;
            matrix.M11 = matrix.M11 * num;
            matrix.M12 = matrix.M12 * num;

            matrix.M21 = matrix.M21 * num;
            matrix.M22 = matrix.M22 * num;

            matrix.M31 = matrix.M31 * num;
            matrix.M32 = matrix.M32 * num;

            return matrix;
        }

        /// <summary>
        ///     Calculates the division of a <see cref="Matrix2D" /> by a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2D" /> by.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Divide(ref Matrix2D matrix, float scalar, out Matrix2D result)
        {
            var num = 1f / scalar;
            result.M11 = matrix.M11 * num;
            result.M12 = matrix.M12 * num;

            result.M21 = matrix.M21 * num;
            result.M22 = matrix.M22 * num;

            result.M31 = matrix.M31 * num;
            result.M32 = matrix.M32 * num;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of two <see cref="Matrix2D" />
        ///     s.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator /(Matrix2D matrix1, Matrix2D matrix2)
        {
            matrix1.M11 = matrix1.M11 / matrix2.M11;
            matrix1.M12 = matrix1.M12 / matrix2.M12;

            matrix1.M21 = matrix1.M21 / matrix2.M21;
            matrix1.M22 = matrix1.M22 / matrix2.M22;

            matrix1.M31 = matrix1.M31 / matrix2.M31;
            matrix1.M32 = matrix1.M32 / matrix2.M32;
            return matrix1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the division of a <see cref="Matrix2D" /> by
        ///     a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="scalar">The amount to divide the <see cref="Matrix2D" /> by.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator /(Matrix2D matrix, float scalar)
        {
            var num = 1f / scalar;
            matrix.M11 = matrix.M11 * num;
            matrix.M12 = matrix.M12 * num;

            matrix.M21 = matrix.M21 * num;
            matrix.M22 = matrix.M22 * num;

            matrix.M31 = matrix.M31 * num;
            matrix.M32 = matrix.M32 * num;
            return matrix;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the inversion of a <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D Invert(Matrix2D matrix)
        {
            var det = 1 / matrix.Determinant();

            var m11 = matrix.M22 * det;
            var m12 = -matrix.M12 * det;

            var m21 = -matrix.M21 * det;
            var m22 = matrix.M11 * det;

            var m31 = (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det;
            var m32 = -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det;

            return new Matrix2D(m11, m12, m21, m22, m31, m32);
        }

        /// <summary>
        ///     Calculates the inversion of a <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="result">The resulting <see cref="Matrix2D" />.</param>
        public static void Invert(ref Matrix2D matrix, out Matrix2D result)
        {
            var det = 1 / matrix.Determinant();

            result.M11 = matrix.M22 * det;
            result.M12 = -matrix.M12 * det;

            result.M21 = -matrix.M21 * det;
            result.M22 = matrix.M11 * det;

            result.M31 = (matrix.M32 * matrix.M21 - matrix.M31 * matrix.M22) * det;
            result.M32 = -(matrix.M32 * matrix.M11 - matrix.M31 * matrix.M12) * det;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix2D" /> struct with the inversion of a <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <returns>The resulting <see cref="Matrix2D" />.</returns>
        public static Matrix2D operator -(Matrix2D matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;

            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;

            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            return matrix;
        }

        /// <summary>
        ///     Compares a <see cref="Matrix2D" /> for equality with another <see cref="Matrix2D" /> without any tolerance.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns><c>true</c> if the <see cref="Matrix2D" />s are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Matrix2D matrix1, Matrix2D matrix2)
        {
            return (matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12) && (matrix1.M21 == matrix2.M21) &&
                   (matrix1.M22 == matrix2.M22) && (matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32);
        }

        /// <summary>
        ///     Compares a <see cref="Matrix2D" /> for inequality with another <see cref="Matrix2D" /> without any tolerance.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix2D" />.</param>
        /// <param name="matrix2">The second <see cref="Matrix2D" />.</param>
        /// <returns><c>true</c> if the <see cref="Matrix2D" />s are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Matrix2D matrix1, Matrix2D matrix2)
        {
            return (matrix1.M11 != matrix2.M11) || (matrix1.M12 != matrix2.M12) || (matrix1.M21 != matrix2.M21) ||
                   (matrix1.M22 != matrix2.M22) || (matrix1.M31 != matrix2.M31) || (matrix1.M32 != matrix2.M32);
        }

        /// <summary>
        ///     Returns a value that indicates whether the current <see cref="Matrix2D" /> is equal to a specified
        ///     <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" /> with which to make the comparison.</param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="Matrix2D" /> is equal to the specified <see cref="Matrix2D" />;
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Equals(ref Matrix2D matrix)
        {
            return (M11 == matrix.M11) && (M12 == matrix.M12) && (M21 == matrix.M21) && (M22 == matrix.M22) &&
                   (M31 == matrix.M31) && (M32 == matrix.M32);
        }

        /// <summary>
        ///     Returns a value that indicates whether the current <see cref="Matrix2D" /> is equal to a specified
        ///     <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" /> with which to make the comparison.</param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="Matrix2D" /> is equal to the specified <see cref="Matrix2D" />;
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Equals(Matrix2D matrix)
        {
            return Equals(ref matrix);
        }

        /// <summary>
        ///     Returns a value that indicates whether the current <see cref="Matrix2D" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object with which to make the comparison.</param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="Matrix2D" /> is equal to the specified object;
        ///     <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Matrix2D && Equals((Matrix2D)obj);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="Matrix2D" />.
        /// </summary>
        /// <returns>
        ///     A hash code for this <see cref="Matrix2D" />, suitable for use in hashing algorithms and data structures like a
        ///     hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return M11.GetHashCode() + M12.GetHashCode() + M21.GetHashCode() + M22.GetHashCode() + M31.GetHashCode() +
                   M32.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Matrix2D" /> to <see cref="Matrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <returns>
        ///     The resulting <see cref="Matrix" />.
        /// </returns>
        public static implicit operator Matrix(Matrix2D matrix)
        {
            return new Matrix(matrix.M11, matrix.M12, 0, 0, matrix.M21, matrix.M22, 0, 0, 0, 0, 1, 0, matrix.M31,
                matrix.M32, 0, 1);
        }

        /// <summary>
        ///     Performs an explicit conversion from a specified <see cref="Matrix2D" /> to a <see cref="Matrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2D" />.</param>
        /// <param name="depth">The depth value.</param>
        /// <param name="result">The resulting <see cref="Matrix" />.</param>
        public static void ToMatrix(ref Matrix2D matrix, float depth, out Matrix result)
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
        ///     Performs an explicit conversion from a specified <see cref="Matrix2D" /> to a <see cref="Matrix" />.
        /// </summary>
        /// <param name="depth">The depth value.</param>
        /// <returns>The resulting <see cref="Matrix" />.</returns>
        public Matrix ToMatrix(float depth = 0)
        {
            Matrix result;
            ToMatrix(ref this, depth, out result);
            return result;
        }

        /// <summary>
        ///     Gets the debug display string.
        /// </summary>
        /// <value>
        ///     The debug display string.
        /// </value>
        internal string DebugDisplayString => this == Identity
                ? "Identity"
                : $"T:({Translation.X:0.##},{Translation.Y:0.##}), R:{MathHelper.ToDegrees(Rotation):0.##}°, S:({Scale.X:0.##},{Scale.Y:0.##})"
            ;

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Matrix2D" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Matrix2D" />.
        /// </returns>
        public override string ToString()
        {
            return $"{{M11:{M11} M12:{M12}}} {{M21:{M21} M22:{M22}}} {{M31:{M31} M32:{M32}}}";
        }
    }
}