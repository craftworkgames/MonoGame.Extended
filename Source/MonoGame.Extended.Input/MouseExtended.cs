﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    public static class MouseExtended
    {
        private static MouseState _currentMouseState;
        private static MouseState _previousMouseState;

        public static MouseStateExtended GetState()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
            return new MouseStateExtended(_currentMouseState, _previousMouseState);
        }

        public static void SetPosition(int x, int y) => Mouse.SetPosition(x, y);
        public static void SetPosition(Point point) => Mouse.SetPosition(point.X, point.Y);
        public static void SetCursor(MouseCursor cursor) => Mouse.SetCursor(cursor);

        public static IntPtr WindowHandle
        {
            get { return Mouse.WindowHandle; }
            set { Mouse.WindowHandle = value; }
        }
    }
}