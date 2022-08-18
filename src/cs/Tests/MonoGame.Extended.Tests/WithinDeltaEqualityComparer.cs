using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tests;

public class WithinDeltaEqualityComparer : IEqualityComparer<float>
{
    private readonly float _delta;

    public WithinDeltaEqualityComparer(float delta)
    {
        _delta = delta;
    }

    public bool Equals(float x, float y)
    {
        return Math.Abs(x - y) < _delta;
    }

    public int GetHashCode(float obj)
    {
        return obj.GetHashCode();
    }
}
