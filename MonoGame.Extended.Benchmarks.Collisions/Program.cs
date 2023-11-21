using BenchmarkDotNet.Running;
using MonoGame.Extended.Benchmarks.Collisions;

var summary = BenchmarkRunner.Run<DifferentPoolSizeCollision>();
