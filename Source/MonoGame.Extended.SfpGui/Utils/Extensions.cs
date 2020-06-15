using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoGame.Extended.SfpGui.Utils {
    public static class Extensions {
        public static Size2 ToSize (this Vector2 v) {
            return new Size2 ((int) v.X, (int) v.Y);
        }
    }
}