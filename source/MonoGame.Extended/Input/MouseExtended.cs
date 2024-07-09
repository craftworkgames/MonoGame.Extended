﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input;

public static class MouseExtended
{
    private static MouseState _currentMouseState;
    private static MouseState _previousMouseState;

    public static MouseStateExtended GetState()
    {
        return new MouseStateExtended(_currentMouseState, _previousMouseState);
    }

    public static void Update()
    {
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    public static void SetPosition(int x, int y) => Mouse.SetPosition(x, y);
    public static void SetPosition(Point point) => Mouse.SetPosition(point.X, point.Y);
#if !FNA
    public static void SetCursor(MouseCursor cursor) => Mouse.SetCursor(cursor);
#endif

    public static IntPtr WindowHandle
    {
        get => Mouse.WindowHandle;
        set => Mouse.WindowHandle = value;
    }
}
