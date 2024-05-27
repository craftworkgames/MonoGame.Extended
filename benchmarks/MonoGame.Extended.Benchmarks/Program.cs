using BenchmarkDotNet.Running;
using MonoGame.Extended.Benchmarks;


BenchmarkRunner.Run<Matrix3x2Benchmarks>();

Console.WriteLine("finished");
