using System;
using MonoGame.Extended.Collisions.Layers;

namespace MonoGame.Extended.Collisions;

internal readonly struct LayerPair : IEquatable<LayerPair>
{
    public readonly Layer First;
    public readonly Layer Second;

    public LayerPair(Layer first, Layer second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        if (ReferenceEquals(first, second) || first.GetHashCode() <= second.GetHashCode())
        {
            First = first;
            Second = second;
        }
        else
        {
            First = second;
            Second = first;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is LayerPair other && Equals(other);
    }

    public readonly bool Equals(LayerPair other)
    {
        return ReferenceEquals(First, other.First) && ReferenceEquals(Second, other.Second);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(First, Second);
    }
}
