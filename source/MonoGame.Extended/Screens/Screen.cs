using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens;

/// <summary>
/// Represents an abstract base class for game screens that can be managed by a <see cref="ScreenManager"/>.
/// </summary>
/// <remarks>
/// <para>
///     Screens provide a way to organize game logic into discrete states such as menus, gameplay, loading screens,
///     or different game areas.
/// </para>
/// <para>
///     When screen are used with a <see cref="Screens.ScreenManager"/>, multiple screens can be active
///     simultaneously in the manager's screen stack based on their <see cref="UpdateWhenInactive"/> and
///     <see cref="DrawWhenInactive"/> properties.
/// </para>
/// </remarks>
public abstract class Screen : IDisposable
{
    /// <summary>
    /// Gets the <see cref="Screens.ScreenManager"/> that manages this screen or <see langword="null"/> if the
    /// screen is not managed.
    /// </summary>
    public ScreenManager ScreenManager { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this screen is currently the active screen.
    /// </summary>
    public bool IsActive { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this screen should continue to update when it is not the active screen.
    /// </summary>
    /// <remarks>
    /// This property allows background screens to continue processing logic even when they are not the topmost
    /// screen in the screen stack of a screen manager.  This is useful for scenarios were persistent game systems
    /// should run regardless of screen state.
    /// </remarks>
    public bool UpdateWhenInactive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this screen should continue to draw when it is not the active screen.
    /// </summary>
    /// <remarks>
    /// This property allows background screens to continue rendering even when they are not the topmost screen
    /// in the screen stack of a screen manager.  This is useful for creating layered visual effects where multiple
    /// screens nee dto be visible simultaneously such as a game world visible behind a semi-transparent menu or
    /// overlay.
    /// </remarks>
    public bool DrawWhenInactive { get; set; }

    /// <summary>
    /// Releases all resources used by the screen.
    /// </summary>
    /// <remarks>
    /// The base implementation does nothing.
    /// </remarks>
    public virtual void Dispose() { }

    /// <summary>
    /// Initializes the screen.
    /// </summary>
    /// <remarks>
    /// When the screen is added to a <see cref="Screens.ScreenManager"/>, this method will be called automatically
    /// when the screen is first shown.
    /// The base implementation does nothing.
    /// </remarks>
    public virtual void Initialize() { }

    /// <summary>
    /// Loads content and resources needed by the screen.
    /// </summary>
    /// <remarks>
    /// When the screen is added to a <see cref="Screens.ScreenManager"/>, this method will be called automatically
    /// after <see cref="Initialize()"/> when the screen is first shown.
    /// The base implementation does nothing.
    /// </remarks>
    public virtual void LoadContent() { }

    /// <summary>
    /// Unloads content and resources used by the screen.
    /// </summary>
    /// <remarks>
    /// When the screen is added to a <see cref="Screens.ScreenManager"/>, this method is called automatically when
    /// the screen is closed.
    /// The base implementation does nothing.
    /// </remarks>
    public virtual void UnloadContent() { }

    /// <summary>
    /// Updates the screen's logic.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <remarks>
    /// When added to a <see cref="Screens.ScreenManager"/>, this method is called automatically every frame when
    /// either the <see cref="IsActive"/> or <see cref="UpdateWhenInactive"/> properties are <see langword="true"/>.
    /// </remarks>
    public abstract void Update(GameTime gameTime);

    /// <summary>
    /// Draws the screen's visual content.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <remarks>
    /// When added to a <see cref="Screens.ScreenManager"/>, this method is called automatically every frame when
    /// either the <see cref="IsActive"/> or <see cref="DrawWhenInactive"/> properties are <see langword="true"/>.
    /// </remarks>
    public abstract void Draw(GameTime gameTime);
}
