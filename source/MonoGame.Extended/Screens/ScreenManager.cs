using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens.Transitions;

namespace MonoGame.Extended.Screens;

/// <summary>
/// Manages a stack of <see cref="Screen"/> instances in a game.
/// </summary>
/// <remarks>
/// <para>
///     Screens are managed using a stack based approach where the topmost screen is considered the active screen,
///     while underlying screens can continue to update and draw in the background based on their settings.
/// </para>
/// <para>
///     Note: Developers should be aware of performance considerations when keeping multiple screens active in the
///     background for updates and/or drawing.
/// </para>
/// </remarks>
public class ScreenManager : SimpleDrawableGameComponent
{
    private readonly Stack<Screen> _screens;
    private Screen _activeScreen;
    private Transition _activeTransition;
    private Screen[] _cachedScreens;
    private bool _isScreenArrayDirty;

    /// <summary>
    /// Gets the currently active screen at the top of the screen stack,
    /// or <see langword="null"/> if no screens are loaded.
    /// </summary>
    public Screen ActiveScreen => _activeScreen;

    /// <summary>
    /// Gets a read-only list of all screens in the stack, ordered from bottom to top.
    /// </summary>
    /// <remarks>
    /// For performance, the list is cached internally and only rebuilt when the screen stack has been modified.
    /// </remarks>
    public IReadOnlyList<Screen> Screens
    {
        get
        {
            if (_isScreenArrayDirty)
            {
                _cachedScreens = _screens.Reverse().ToArray();
                _isScreenArrayDirty = false;
            }

            return _cachedScreens;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenManager"/> class.
    /// </summary>
    public ScreenManager()
    {
        _screens = new Stack<Screen>();
        _cachedScreens = Array.Empty<Screen>();
        _isScreenArrayDirty = true;
    }

    /// <summary>
    /// Pushes a new screen onto the stack and makes it the active screen.
    /// </summary>
    /// <remarks>
    /// The previous active screen remains in the screen stack but becomes inactive.
    /// </remarks>
    /// <param name="screen">The screen to show.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="screen"/> is <see langword="null"/>.</exception>
    public void ShowScreen(Screen screen)
    {
        ArgumentNullException.ThrowIfNull(screen);

        if (_activeScreen != null)
        {
            _activeScreen.IsActive = false;
        }

        screen.ScreenManager = this;
        screen.IsActive = true;
        screen.Initialize();
        screen.LoadContent();

        _screens.Push(screen);
        _activeScreen = screen;
        _isScreenArrayDirty = true;
    }

    /// <summary>
    /// Pushes a new screen onto the stack with a transition effect and makes it the active screen.
    /// </summary>
    /// <remarks>
    /// The previous active screen remains in the screen stack but becomes inactive.
    /// </remarks>
    /// <param name="screen">The screen to show.</param>
    /// <param name="transition">The transition effect to use when showing the screen.</param>
    public void ShowScreen(Screen screen, Transition transition)
    {
        if (_activeTransition != null)
        {
            return;
        }

        _activeTransition = transition;
        _activeTransition.StateChanged += (sender, args) => ShowScreen(screen);
        _activeTransition.Completed += (sender, args) =>
        {
            _activeTransition.Dispose();
            _activeTransition = null;
        };
    }

    /// <summary>
    /// Pops the current active screen from the stack, disposing it and making the next screen active.
    /// </summary>
    /// <remarks>
    /// If the screen stack becomes empty, no screen will be active.
    /// </remarks>
    public void CloseScreen()
    {
        if (!_screens.TryPop(out Screen screen))
        {
            return;
        }

        screen.IsActive = false;
        screen.UnloadContent();
        screen.Dispose();

        _isScreenArrayDirty = true;

        if (_screens.TryPeek(out _activeScreen))
        {
            _activeScreen.IsActive = true;
        }
    }

    /// <summary>
    /// Pops the current active screen from the stack with a transition effect, disposing it and making the next screen active.
    /// </summary>
    /// <remarks>
    /// If the screen stack becomes empty, no screen will be active.
    /// </remarks>
    /// <param name="transition">The transition effect to use when closing the screen.</param>
    public void CloseScreen(Transition transition)
    {
        if (_activeTransition != null)
        {
            return;
        }

        _activeTransition = transition;
        _activeTransition.StateChanged += (sender, args) => CloseScreen();
        _activeTransition.Completed += (sender, args) =>
        {
            _activeTransition.Dispose();
            _activeTransition = null;
        };
    }

    /// <summary>
    /// Replaces the current active screen with a new screen by closing the current screen and showing the new one.
    /// </summary>
    /// <remarks>
    /// This is equivalent to calling <see cref="CloseScreen()"/> followed by <see cref="ShowScreen(Screen)"/>
    /// </remarks>
    /// <param name="screen">The screen to replace the current active screen with.</param>
    public void ReplaceScreen(Screen screen)
    {
        CloseScreen();
        ShowScreen(screen);
    }

    /// <summary>
    /// Replaces the current active screen with a new screen using a transition effect.
    /// </summary>
    /// <remarks>
    /// This is equivalent to calling <see cref="CloseScreen(Transition)"/> followed by <see cref="ShowScreen(Screen, Transition)"/>.
    /// </remarks>
    /// <param name="screen">The screen to replace the current active screen with.</param>
    /// <param name="transition">The transition effect to use when replacing the screen.</param>
    public void ReplaceScreen(Screen screen, Transition transition)
    {
        if (_activeTransition != null)
        {
            return;
        }

        _activeTransition = transition;
        _activeTransition.StateChanged += (sender, args) => ReplaceScreen(screen);
        _activeTransition.Completed += (sender, args) =>
        {
            _activeTransition.Dispose();
            _activeTransition = null;
        };
    }
    /// <summary>
    /// Loads a screen, replacing any existing screens.
    /// </summary>
    /// <param name="screen">The screen to load</param>
    [Obsolete("This method is provided for backward compatibility and will be removed in the next major release. For new code, use ShowScreen(Screen), CloseScreen(), or ReplaceScreen(Screen).")]
    public void LoadScreen(Screen screen)
    {
        ReplaceScreen(screen);
    }

    /// <summary>
    /// Loads a screen with a transition effect, replacing any existing screens.
    /// </summary>
    /// <param name="screen">The screen to load</param>
    /// <param name="transition">The transition effect to use.</param>
    [Obsolete("This method is provided for backward compatibility and will be removed in the next major release. For new code, use ShowScreen(Screen), CloseScreen(), or ReplaceScreen(Screen).")]
    public void LoadScreen(Screen screen, Transition transition)
    {
        ReplaceScreen(screen, transition);
    }

    /// <summary>
    /// Clears all screens from the stack, disposing them and setting no active screen.
    /// </summary>
    public void ClearScreens()
    {
        while (_screens.TryPop(out Screen screen))
        {
            screen.IsActive = false;
            screen.UnloadContent();
            screen.Dispose();
        }

        _activeScreen = null;
        _isScreenArrayDirty = true;
    }

    /// <summary>
    /// Initializes the screen manager and the currently active screen if one exists.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        if (_activeScreen != null)
        {
            _activeScreen.Initialize();
        }
    }

    /// <summary>
    /// Loads content for the screen manager and the currently active screen if one exists.
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();

        if (_activeScreen != null)
        {
            _activeScreen.LoadContent();
        }
    }

    /// <summary>
    /// Unloads content for the screen manager and the currently active screen if one exists.
    /// </summary>
    protected override void UnloadContent()
    {
        base.UnloadContent();

        if (_activeScreen != null)
        {
            _activeScreen.UnloadContent();
        }
    }

    /// <summary>
    /// Updates all screens in the stack where the <see cref="Screen.IsActive"/> or
    /// <see cref="Screen.UpdateWhenInactive"/> property is set to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Update order for screens is done from the bottom of the stack to the top of.
    /// </remarks>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update(GameTime gameTime)
    {
        IReadOnlyList<Screen> screens = Screens;
        for (int i = 0; i < screens.Count; i++)
        {
            Screen screen = screens[i];
            if (screen.IsActive || screen.UpdateWhenInactive)
            {
                screen.Update(gameTime);
            }
        }

        if (_activeTransition != null)
        {
            _activeTransition.Update(gameTime);
        }
    }

    /// <summary>
    /// Draws all screens in the stack where the <see cref="Screen.IsActive"/> or
    /// <see cref="Screen.DrawWhenInactive"/> property is set to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// Draw order for screens is done from the bottom of the stack to the top to allow background screens
    /// to draw before foreground ones for property visual stacking.
    /// </remarks>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Draw(GameTime gameTime)
    {
        IReadOnlyList<Screen> screens = Screens;
        for (int i = 0; i < screens.Count; i++)
        {
            Screen screen = screens[i];
            if (screen.IsActive || screen.DrawWhenInactive)
            {
                screen.Draw(gameTime);
            }
        }

        if (_activeTransition != null)
        {
            _activeTransition.Draw(gameTime);
        }
    }
}

