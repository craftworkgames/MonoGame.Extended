using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input;

/// <summary>
/// Represents mouse input.
/// </summary>
/// <remarks>
/// This si an extended version of the default <see cref="Microsoft.Xna.Framework.Input.Mouse"/> class which offers
/// internal tracking of both the previous and current state of mouse input.
/// </remarks>
public static class MouseExtended
{
    private static MouseState _currentMouseState;
    private static MouseState _previousMouseState;

    /// <summary>
    /// Gets the state of mouse input.
    /// </summary>
    /// <returns>
    /// A <see cref="MouseStateExtended"/> value that represents the state of mouse input.
    /// </returns>
    public static MouseStateExtended GetState()
    {
        return new MouseStateExtended(_currentMouseState, _previousMouseState);
    }

    /// <summary>
    /// Updates the <see cref="MouseExtended"/>.
    /// </summary>
    /// <remarks>
    /// This internally will overwrite the source data for the previous state with the current state, then get the
    /// current state from the mouse input.  This should only be called once per update cycle.  Calling it more than
    /// once per update cycle can result in the cached previous state being overwritten with invalid data.
    /// </remarks>
    public static void Update()
    {
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Sets the position of the mouse cursor to the specified coordinates relative to the game window.
    /// </summary>
    /// <param name="x">The x-coordinate position.</param>
    /// <param name="y">The y-coordinate position.</param>
    public static void SetPosition(int x, int y) => Mouse.SetPosition(x, y);

    /// <summary>
    /// Sets the position of the mouse cursor to the specified coordinate relative to the game window.
    /// </summary>
    /// <param name="point">A <see cref="Point"/> value that represents the x- and y-coordinate positions.</param>
    public static void SetPosition(Point point) => Mouse.SetPosition(point.X, point.Y);

#if !FNA
    /// <summary>
    /// Sets the cursor of the mouse.
    /// </summary>
    /// <param name="cursor">The cursor to use.</param>
    public static void SetCursor(MouseCursor cursor) => Mouse.SetCursor(cursor);
#endif

    /// <summary>
    /// Gets or Sets the window handle of the mouse.
    /// </summary>
    public static IntPtr WindowHandle
    {
        get => Mouse.WindowHandle;
        set => Mouse.WindowHandle = value;
    }
}
