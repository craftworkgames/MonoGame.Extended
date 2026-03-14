using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace MonoGame.Extended.Tests.Screens;

public class TestScreen : Screen
{
    public bool DisposeCalled { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DrawCallCount { get; private set; }
    public int ActivatedCallCount { get; private set; }
    public int DeactivatedCallCount { get; private set; }
    public GameTime LastUpdateGameTime { get; private set; }
    public GameTime LastDrawGameTime { get; private set; }

    public string Name { get; set; }

    // Events for tracking order
    public event Action<string> OnUpdate;
    public event Action<string> OnDraw;

    public TestScreen(string name = "TestScreen")
    {
        Name = name;
    }

    public override void Dispose()
    {
        DisposeCalled = true;
        base.Dispose();
    }

    public override void OnActivated()
    {
        ActivatedCallCount++;
    }

    public override void OnDeactivated()
    {
        DeactivatedCallCount++;
    }

    public override void Update(GameTime gameTime)
    {
        OnUpdate?.Invoke(Name);
        UpdateCallCount++;
        LastUpdateGameTime = gameTime;
    }

    public override void Draw(GameTime gameTime)
    {
        OnDraw?.Invoke(Name);
        DrawCallCount++;
        LastDrawGameTime = gameTime;
    }

    public void Reset()
    {
        DisposeCalled = false;
        UpdateCallCount = 0;
        DrawCallCount = 0;
        ActivatedCallCount = 0;
        DeactivatedCallCount = 0;
        LastUpdateGameTime = null;
        LastDrawGameTime = null;
        OnUpdate = null;
        OnDraw = null;
    }
}

public class TestGameScreen : GameScreen
{
    public TestGameScreen(Game game) : base(game) { }
    public override void Update(GameTime gameTime) { }
    public override void Draw(GameTime gameTime) { }
}
