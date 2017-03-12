using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonoGame.Extended.Tests
{
    public class MockGameWindow : GameWindow
    {
        public override bool AllowUserResizing { get; set; }
        public override Rectangle ClientBounds { get; }
        public override DisplayOrientation CurrentOrientation { get; }
        public override IntPtr Handle { get; }
        public override string ScreenDeviceName { get; }

        public MockGameWindow()
        {
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
        }

        protected override void SetSupportedOrientations(DisplayOrientation orientations)
        {
        }

        protected override void SetTitle(string title)
        {
        }

#if __MonoCS__
        public override Icon Icon { get; set; }
#endif
    }
}