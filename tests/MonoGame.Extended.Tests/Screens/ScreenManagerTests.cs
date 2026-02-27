using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace MonoGame.Extended.Tests.Screens;

[Collection("GraphicsTest")]
public sealed class ScreenManagerTests
{
    #region ShowScreen Tests

    [Fact]
    public void ShowScreen_FirstScreen_BecomesActive()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");

        manager.ShowScreen(screen);

        Assert.Equal(screen, manager.ActiveScreen);
        Assert.True(screen.IsActive);
        Assert.Single(manager.Screens);
    }

    [Fact]
    public void ShowScreen_SecondScreen_DeactivatesFirstAndBecomesActive()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        Assert.Equal(screen2, manager.ActiveScreen);
        Assert.True(screen2.IsActive);
        Assert.False(screen1.IsActive);
        Assert.Equal(2, manager.Screens.Count);
    }

    [Fact]
    public void ShowScreen_SetsScreenManagerProperty()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");

        manager.ShowScreen(screen);

        Assert.Equal(manager, screen.ScreenManager);
    }

    [Fact]
    public void ShowScreen_WithNull_ThrowsArgumentNullException()
    {
        ScreenManager manager = new ScreenManager();
        Assert.Throws<ArgumentNullException>(() => manager.ShowScreen(null));
    }

    #endregion

    #region CloseScreen Tests

    [Fact]
    public void CloseScreen_RemovesActiveAndActivatesPrevious()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.CloseScreen();

        Assert.Equal(screen1, manager.ActiveScreen);
        Assert.True(screen1.IsActive);
        Assert.True(screen2.DisposeCalled);
        Assert.Single(manager.Screens);
    }

    [Fact]
    public void CloseScreen_OnLastScreen_LeavesEmptyStack()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");
        manager.ShowScreen(screen);

        manager.CloseScreen();

        Assert.Null(manager.ActiveScreen);
        Assert.Empty(manager.Screens);
        Assert.True(screen.DisposeCalled);
    }

    [Fact]
    public void CloseScreen_OnEmptyStack_DoesNotThrow()
    {
        ScreenManager manager = new ScreenManager();

        manager.CloseScreen();

        Assert.Null(manager.ActiveScreen);
        Assert.Empty(manager.Screens);
    }

    #endregion

    #region ReplaceScreen Tests

    [Fact]
    public void ReplaceScreen_ClosesActiveAndShowsNew()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        manager.ShowScreen(screen1);

        manager.ReplaceScreen(screen2);

        Assert.Equal(screen2, manager.ActiveScreen);
        Assert.True(screen2.IsActive);
        Assert.True(screen1.DisposeCalled);
        Assert.Single(manager.Screens);
    }

    [Fact]
    public void ReplaceScreen_OnEmptyStack_JustShowsScreen()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");

        manager.ReplaceScreen(screen);

        Assert.Equal(screen, manager.ActiveScreen);
        Assert.Single(manager.Screens);
    }

    [Fact]
    public void ReplaceScreen_WithNull_ThrowsArgumentNullException()
    {
        ScreenManager manager = new ScreenManager();
        Assert.Throws<ArgumentNullException>(() => manager.ReplaceScreen(null));
    }

    #endregion

    #region ClearScreens Tests

    [Fact]
    public void ClearScreens_DisposesAllScreensAndClearsStack()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        TestScreen screen3 = new TestScreen("Screen3");

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);
        manager.ShowScreen(screen3);

        manager.ClearScreens();

        Assert.Null(manager.ActiveScreen);
        Assert.Empty(manager.Screens);
        Assert.True(screen1.DisposeCalled);
        Assert.True(screen2.DisposeCalled);
        Assert.True(screen3.DisposeCalled);
    }

    [Fact]
    public void ClearScreens_OnEmptyStack_DoesNotThrow()
    {
        ScreenManager manager = new ScreenManager();

        manager.ClearScreens();

        Assert.Null(manager.ActiveScreen);
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_OnlyUpdatesActiveScreen_ByDefault()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        GameTime gameTime = new GameTime();

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.Update(gameTime);

        Assert.Equal(0, screen1.UpdateCallCount);
        Assert.Equal(1, screen2.UpdateCallCount);
    }

    [Fact]
    public void Update_UpdatesInactiveScreens_WhenUpdateWhenInactiveIsTrue()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1") { UpdateWhenInactive = true };
        TestScreen screen2 = new TestScreen("Screen2");
        GameTime gameTime = new GameTime();

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.Update(gameTime);

        Assert.Equal(1, screen1.UpdateCallCount);
        Assert.Equal(1, screen2.UpdateCallCount);
    }

    [Fact]
    public void Update_UpdatesScreensInBottomToTopOrder()
    {
        ScreenManager manager = new ScreenManager();
        List<string> updateOrder = new List<string>();
        TestScreen screen1 = new TestScreen("Screen1") { UpdateWhenInactive = true };
        TestScreen screen2 = new TestScreen("Screen2") { UpdateWhenInactive = true };
        TestScreen screen3 = new TestScreen("Screen3");

        screen1.OnUpdate += name => updateOrder.Add(name);
        screen2.OnUpdate += name => updateOrder.Add(name);
        screen3.OnUpdate += name => updateOrder.Add(name);

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);
        manager.ShowScreen(screen3);

        manager.Update(new GameTime());

        Assert.Equal(["Screen1", "Screen2", "Screen3"], updateOrder);
    }

    [Fact]
    public void Update_OnEmptyStack_DoesNotThrow()
    {
        ScreenManager manager = new ScreenManager();

        manager.Update(new GameTime());
    }

    #endregion

    #region Draw Tests

    [Fact]
    public void Draw_OnlyDrawsActiveScreen_ByDefault()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        GameTime gameTime = new GameTime();

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.Draw(gameTime);

        Assert.Equal(0, screen1.DrawCallCount);
        Assert.Equal(1, screen2.DrawCallCount);
    }

    [Fact]
    public void Draw_DrawsInactiveScreens_WhenDrawWhenInactiveIsTrue()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1") { DrawWhenInactive = true };
        TestScreen screen2 = new TestScreen("Screen2");
        GameTime gameTime = new GameTime();

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.Draw(gameTime);

        Assert.Equal(1, screen1.DrawCallCount);
        Assert.Equal(1, screen2.DrawCallCount);
    }

    [Fact]
    public void Draw_DrawsScreensInBottomToTopOrder()
    {
        ScreenManager manager = new ScreenManager();
        List<string> drawOrder = new List<string>();
        TestScreen screen1 = new TestScreen("Screen1") { DrawWhenInactive = true };
        TestScreen screen2 = new TestScreen("Screen2") { DrawWhenInactive = true };
        TestScreen screen3 = new TestScreen("Screen3");

        screen1.OnDraw += name => drawOrder.Add(name);
        screen2.OnDraw += name => drawOrder.Add(name);
        screen3.OnDraw += name => drawOrder.Add(name);

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);
        manager.ShowScreen(screen3);

        manager.Draw(new GameTime());

        Assert.Equal(["Screen1", "Screen2", "Screen3"], drawOrder);
    }

    [Fact]
    public void Draw_OnEmptyStack_DoesNotThrow()
    {
        ScreenManager manager = new ScreenManager();

        manager.Draw(new GameTime());
    }

    #endregion

    #region OnActivated / OnDeactivated Tests

    [Fact]
    public void ShowScreen_FirstScreen_CallsOnActivated()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");

        manager.ShowScreen(screen);

        Assert.Equal(1, screen.ActivatedCallCount);
        Assert.Equal(0, screen.DeactivatedCallCount);
    }

    [Fact]
    public void ShowScreen_SecondScreen_CallsOnDeactivatedOnFirstAndOnActivatedOnSecond()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        Assert.Equal(1, screen1.ActivatedCallCount);
        Assert.Equal(1, screen1.DeactivatedCallCount);
        Assert.Equal(1, screen2.ActivatedCallCount);
        Assert.Equal(0, screen2.DeactivatedCallCount);
    }

    [Fact]
    public void CloseScreen_CallsOnDeactivatedOnClosedScreen_AndOnActivatedOnRevealed()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);

        manager.CloseScreen();

        Assert.Equal(1, screen2.DeactivatedCallCount);
        Assert.Equal(2, screen1.ActivatedCallCount); // once on show, once on re-activation
    }

    [Fact]
    public void CloseScreen_LastScreen_CallsOnDeactivatedButNotOnActivated()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen = new TestScreen("Screen1");
        manager.ShowScreen(screen);

        manager.CloseScreen();

        Assert.Equal(1, screen.DeactivatedCallCount);
        Assert.Equal(1, screen.ActivatedCallCount);
    }

    [Fact]
    public void ClearScreens_CallsOnDeactivatedOnlyOnActiveScreen()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        TestScreen screen3 = new TestScreen("Screen3");
        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);
        manager.ShowScreen(screen3);

        // Snapshot counts before clear — inactive screens already received
        // OnDeactivated when a new screen was pushed on top of them.
        int screen1CountBefore = screen1.DeactivatedCallCount;
        int screen2CountBefore = screen2.DeactivatedCallCount;

        manager.ClearScreens();

        // ClearScreens must not trigger additional OnDeactivated on already-inactive screens.
        Assert.Equal(screen1CountBefore, screen1.DeactivatedCallCount);
        Assert.Equal(screen2CountBefore, screen2.DeactivatedCallCount);
        Assert.Equal(1, screen3.DeactivatedCallCount);
    }

    #endregion

    #region Stack Ordering Tests

    [Fact]
    public void Screens_ReturnsScreensInBottomToTopOrder()
    {
        ScreenManager manager = new ScreenManager();
        TestScreen screen1 = new TestScreen("Screen1");
        TestScreen screen2 = new TestScreen("Screen2");
        TestScreen screen3 = new TestScreen("Screen3");

        manager.ShowScreen(screen1);
        manager.ShowScreen(screen2);
        manager.ShowScreen(screen3);

        IReadOnlyList<Screen> screens = manager.Screens;
        Assert.Equal(3, screens.Count);
        Assert.Equal(screen1, screens[0]);
        Assert.Equal(screen2, screens[1]);
        Assert.Equal(screen3, screens[2]);
    }

    #endregion

    #region Cache Invalidation Tests

    [Fact]
    public void Screens_CachesResultsBetweenCalls()
    {
        ScreenManager manager = new ScreenManager();
        manager.ShowScreen(new TestScreen("Screen1"));

        IReadOnlyList<Screen> screens1 = manager.Screens;
        IReadOnlyList<Screen> screens2 = manager.Screens;

        Assert.Same(screens1, screens2);
    }

    [Fact]
    public void Screens_InvalidatesCacheOnShowScreen()
    {
        ScreenManager manager = new ScreenManager();
        manager.ShowScreen(new TestScreen("Screen1"));
        IReadOnlyList<Screen> screensBefore = manager.Screens;

        manager.ShowScreen(new TestScreen("Screen2"));
        IReadOnlyList<Screen> screensAfter = manager.Screens;

        Assert.NotSame(screensBefore, screensAfter);
        Assert.Equal(2, screensAfter.Count);
    }

    [Fact]
    public void Screens_InvalidatesCacheOnCloseScreen()
    {
        ScreenManager manager = new ScreenManager();
        manager.ShowScreen(new TestScreen("Screen1"));
        manager.ShowScreen(new TestScreen("Screen2"));
        IReadOnlyList<Screen> screensBefore = manager.Screens;

        manager.CloseScreen();
        IReadOnlyList<Screen> screensAfter = manager.Screens;

        Assert.NotSame(screensBefore, screensAfter);
        Assert.Single(screensAfter);
    }

    [Fact]
    public void Screens_InvalidatesCacheOnClearScreens()
    {
        ScreenManager manager = new ScreenManager();
        manager.ShowScreen(new TestScreen("Screen1"));
        IReadOnlyList<Screen> screensBefore = manager.Screens;

        manager.ClearScreens();
        IReadOnlyList<Screen> screensAfter = manager.Screens;

        Assert.NotSame(screensBefore, screensAfter);
        Assert.Empty(screensAfter);
    }

    #endregion
}
