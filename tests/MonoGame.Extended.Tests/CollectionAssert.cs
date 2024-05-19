using System.Collections.Generic;
using Xunit;

namespace MonoGame.Extended.Tests;

public static class CollectionAssert
{
    public static void Equal<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual)
    {
        Assert.True(expected.Count == actual.Count, "The number of items in the collections does not match.");

        Assert.All(actual, x => Assert.Contains(x, expected));
    }
}
