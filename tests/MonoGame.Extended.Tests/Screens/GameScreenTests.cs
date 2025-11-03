using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tests.Fixtures;

namespace MonoGame.Extended.Tests.Screens;

public sealed class GameScreenTests
{
    [Fact]
    public void Constructor_WithNullGame_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TestGameScreen(null));
    }
}
