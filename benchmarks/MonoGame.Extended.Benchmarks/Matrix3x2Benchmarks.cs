using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Benchmarks;

[MemoryDiagnoser]
public class Matrix3x2Benchmarks
{
    private Matrix2 _matrix2;
    private Matrix3x2 _matrix3x2;

    [GlobalSetup]
    public void Setup()
    {
        _matrix2 = new Matrix2(1, 2, 3, 4, 5, 6);
        _matrix3x2 = new Matrix3x2(1, 2, 3, 4, 5, 6);
    }

    [Benchmark]
    public Vector2 Matrix2_getTranslation() => _matrix2.Translation;

    [Benchmark]
    public Vector2 Matrix3x2_getTranslation() => _matrix3x2.Translation;

    // [Benchmark]
    // public float Matrix2_getRotation() => _matrix2.Rotation;

    // [Benchmark]
    // public float Matrix3x2_getRotation() => _matrix3x2.Rotation;

    // [Benchmark]
    // public Vector2 Matrix2_getScale() => _matrix2.Scale;

    // [Benchmark]
    // public Vector2 Matrix3x2_getScale() => _matrix3x2.Scale;

    // [Benchmark]
    // public (Vector2, float, Vector2) Matrix2_Decompose()
    // {
    //     Vector2 translation = _matrix2.Translation;
    //     float rotation = _matrix2.Rotation;
    //     Vector2 scale = _matrix2.Scale;
    //     return (translation, rotation, scale);
    // }

    // [Benchmark]
    // public (Vector2, float, Vector2) Matrix3x2_Decompose()
    // {
    //     _matrix3x2.Decompose(out Vector2 translation, out float rotation, out Vector2 scale);
    //     return (translation, rotation, scale);
    // }

    // [Benchmark]
    // public Vector2 Matrix2_Transform() => _matrix2.Transform(Vector2.One);

    // [Benchmark]
    // public Vector2 Matrix3x2_Transform() => _matrix3x2.Transform(Vector2.One);

    // [Benchmark]
    // public float Matrix2_Determinant() => _matrix2.Determinant();

    // [Benchmark]
    // public float Matrix3x2_Determinant() => _matrix3x2.Determinant();

    // [Benchmark]
    // public Matrix2 Matrix2_CreateFrom()
    // {
    //     Matrix2.CreateFrom(Vector2.Zero, 1.0f, Vector2.One, Vector2.Zero, out Matrix2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_CreateFrom()
    // {
    //     Matrix3x2.CreateFrom(Vector2.Zero, 1.0f, Vector2.One, Vector2.Zero, out Matrix3x2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_CreateRotationZ()
    // {
    //     Matrix2.CreateRotationZ(1.0f, out Matrix2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_CreateRotationZ()
    // {
    //     Matrix3x2.CreateRotationZ(1.0f, out Matrix3x2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_CreateScale()
    // {
    //     Matrix2.CreateScale(1.0f, out Matrix2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_CreateScale()
    // {
    //     Matrix3x2.CreateScale(1.0f, out Matrix3x2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_CreateTranslation()
    // {
    //     Matrix2.CreateTranslation(1.0f, 1.0f, out Matrix2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_CreateTranslation()
    // {
    //     Matrix3x2.CreateTranslation(1.0f, 1.0f, out Matrix3x2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_Invert()
    // {
    //     Matrix2.Invert(ref _matrix2, out Matrix2 result);
    //     return result;
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_Invert()
    // {
    //     Matrix3x2.Invert(_matrix3x2);
    //     return _matrix3x2;
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_Add()
    // {
    //     return Matrix2.Add(_matrix2, _matrix2);
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_Add()
    // {
    //     return Matrix3x2.Add(_matrix3x2, _matrix3x2);
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_Subtract()
    // {
    //     return Matrix2.Subtract(_matrix2, _matrix2);
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_Subtract()
    // {
    //     return Matrix3x2.Subtract(_matrix3x2, _matrix3x2);
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_Multiply()
    // {
    //     return Matrix2.Subtract(_matrix2, _matrix2);
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_Multiply()
    // {
    //     return Matrix3x2.Multiply(_matrix3x2, _matrix3x2);
    // }

    // [Benchmark]
    // public Matrix2 Matrix2_Divide()
    // {
    //     return Matrix2.Divide(_matrix2, _matrix2);
    // }

    // [Benchmark]
    // public Matrix3x2 Matrix3x2_Divide()
    // {
    //     return Matrix3x2.Divide(_matrix3x2, _matrix3x2);
    // }

    // [Benchmark]
    // public Matrix Matrix2_ToMatrix()
    // {
    //     return _matrix2.ToMatrix();
    // }

    // [Benchmark]
    // public Matrix Matrix3x2_ToMatrix()
    // {
    //     return _matrix3x2.ToMatrix();
    // }
}
