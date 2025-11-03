using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Screens;

/// <summary>
/// Provides an abstract base class for game screens that require access to core game services and components.
/// </summary>
/// <remarks>
/// <para>
///     The <see cref="GameScreen"/> class extends <see cref="Screen"/> by providing convenient access to commonly
///     used game services such as the content manager, graphics device, and service container. This eliminates
///     the need for derived screens to manually access these services through the game instance.
/// </para>
/// </remarks>
public abstract class GameScreen : Screen
{
    /// <summary>
    /// Gets the game instance associated with this screen that provides access to core services and components.
    /// </summary>
    public Game Game { get; }

    /// <summary>
    /// Gets the content manager for loading game assets.
    /// </summary>
    public ContentManager Content => Game.Content;

    /// <summary>
    /// Gets the graphics device for rendering operations.
    /// </summary>
    public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

    /// <summary>
    /// Gets the service container for accessing registered game services.
    /// </summary>
    public GameServiceContainer Services => Game.Services;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameScreen"/> class.
    /// </summary>
    /// <param name="game">The game instance that provides access to core services.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="game"/> is <see langword="null"/>.
    /// </exception>
    protected GameScreen(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        Game = game;
    }
}
