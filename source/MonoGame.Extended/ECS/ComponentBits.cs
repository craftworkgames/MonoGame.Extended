using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.ECS;

/// <summary>
/// Represents a bit field for tracking which component types are associated with an entity.
/// </summary>
/// <remarks>
/// This structure supports up to 256 different component types (4 segments × 64 bits each), where each bit position
/// corresponds to a specific component type.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public struct ComponentBits : IEquatable<ComponentBits>
{
    private const int BITS_PER_SEGMENT = 64;
    private const int SEGMENT_COUNT = 4;
    private const int MAX_BITS = BITS_PER_SEGMENT * SEGMENT_COUNT;

    private ulong _bits0;
    private ulong _bits1;
    private ulong _bits2;
    private ulong _bits3;

    /// <summary>
    /// Gets or sets a bit at the specified index, representing whether a specific component type is present.
    /// </summary>
    /// <param name="bitIndex">The zero-based index of the bit to get or set. Must be between 0 and 255.</param>
    /// <returns>
    /// <see langword="true"/> if the bit at the specified index is set; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="bitIndex"/> is less than 0 or greater than or equal to 256.
    /// </exception>
    public unsafe bool this[int bitIndex]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bitIndex, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bitIndex, MAX_BITS);

            // bitIndex / BITS_PER_SEGMENT;
            int segmentIndex = bitIndex >> 6;

            // bitIndex % BITS_PER_SEGMENT;
            int bitPosition = bitIndex & 63;

            ulong mask = 1UL << bitPosition;

            ref ulong segment = ref Unsafe.Add(ref _bits0, segmentIndex);
            return (segment & mask) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bitIndex, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bitIndex, MAX_BITS);

            // bitIndex / BITS_PER_SEGMENT;
            int segmentIndex = bitIndex >> 6;

            // bitIndex % BITS_PER_SEGMENT;
            int bitPosition = bitIndex & 63;

            ulong mask = 1UL << bitPosition;

            ref ulong segment = ref Unsafe.Add(ref _bits0, segmentIndex);
            if (value)
            {
                segment |= mask;
            }
            else
            {
                segment &= ~mask;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether no component types are present.
    /// </summary>
    /// <returns><see langword="true"/> if no component types are set; otherwise, <see langword="false"/>.</returns>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bits0 == 0 && _bits1 == 0 && _bits2 == 0 && _bits3 == 0;
    }

    /// <summary>
    /// Determines if all the component types in the required set are present in this component set.
    /// </summary>
    /// <param name="required">The component types that must all be present.</param>
    /// <returns>
    /// <see langword="true"/> if this component set has all the required component types;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasAll(ComponentBits required)
    {
        return (_bits0 & required._bits0) == required._bits0 &&
               (_bits1 & required._bits1) == required._bits1 &&
               (_bits2 & required._bits2) == required._bits2 &&
               (_bits3 & required._bits3) == required._bits3;
    }

    /// <summary>
    /// Determines if this component set contains at least one of the specified component types.
    /// </summary>
    /// <param name="options">The component types to check for.</param>
    /// <returns>
    /// <see langword="true"/> if this component set has at least one of the specified component types;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasAny(ComponentBits options)
    {
        return ((_bits0 & options._bits0) |
                (_bits1 & options._bits1) |
                (_bits2 & options._bits2) |
                (_bits3 & options._bits3)) != 0;
    }

    /// <summary>
    /// Determines if this component set contains none of the specified component types.
    /// </summary>
    /// <param name="excluded">The component types that must not be present.</param>
    /// <returns>
    /// <see langword="true"/> if this component set has none of the specified component types;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool HasNone(ComponentBits excluded)
    {
        return ((_bits0 & excluded._bits0) |
                (_bits1 & excluded._bits1) |
                (_bits2 & excluded._bits2) |
                (_bits3 & excluded._bits3)) == 0;
    }

    /// <summary>
    /// Removes all component types from this component set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _bits0 = _bits1 = _bits2 = _bits3 = 0;

    /// <summary>
    /// Marks all possible component types as present in this component set.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetAll() => _bits0 = _bits1 = _bits2 = _bits3 = ulong.MaxValue;

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object obj) => obj is ComponentBits other && Equals(other);

    /// <inheritdoc />
    public readonly bool Equals(ComponentBits other)
    {
        return _bits0 == other._bits0 &&
               _bits1 == other._bits1 &&
               _bits2 == other._bits2 &&
               _bits3 == other._bits3;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode() => HashCode.Combine(_bits0, _bits1, _bits2, _bits3);

    /// <inheritdoc />
    public static bool operator ==(ComponentBits left, ComponentBits right) => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(ComponentBits left, ComponentBits right) => !left.Equals(right);

    /// <inheritdoc />
    public static ComponentBits operator |(ComponentBits left, ComponentBits right)
    {
        return new ComponentBits()
        {
            _bits0 = left._bits0 | right._bits0,
            _bits1 = left._bits1 | right._bits1,
            _bits2 = left._bits2 | right._bits2,
            _bits3 = left._bits3 | right._bits3
        };
    }

    /// <inheritdoc />
    public static ComponentBits operator &(ComponentBits left, ComponentBits right)
    {
        return new ComponentBits()
        {
            _bits0 = left._bits0 & right._bits0,
            _bits1 = left._bits1 & right._bits1,
            _bits2 = left._bits2 & right._bits2,
            _bits3 = left._bits3 & right._bits3
        };
    }

    /// <inheritdoc />
    public static ComponentBits operator ~(ComponentBits other)
    {
        return new ComponentBits()
        {
            _bits0 = ~other._bits0,
            _bits1 = ~other._bits1,
            _bits2 = ~other._bits2,
            _bits3 = ~other._bits3
        };
    }
}
