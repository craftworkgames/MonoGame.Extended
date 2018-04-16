using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.Gui
{
    public class PointerEventArgs : EventArgs
    {
        private PointerEventArgs()
        {
        }

        public Point Position { get; private set; }
        public MouseButton Button { get; private set; }
        public int ScrollWheelDelta { get; private set; }
        public int ScrollWheelValue { get; private set; }
        public TimeSpan Time { get; private set; }

        public static PointerEventArgs FromMouseArgs(MouseEventArgs args)
        {
            return new PointerEventArgs
            {
                Position = args.Position,
                Button = args.Button,
                ScrollWheelDelta = args.ScrollWheelDelta,
                ScrollWheelValue = args.ScrollWheelValue,
                Time = args.Time
            };
        }

        public static PointerEventArgs FromTouchArgs(TouchEventArgs args)
        {
            return new PointerEventArgs
            {
                Position = args.Position,
                Button = MouseButton.Left,
                Time = args.Time
            };
        }
    }
}