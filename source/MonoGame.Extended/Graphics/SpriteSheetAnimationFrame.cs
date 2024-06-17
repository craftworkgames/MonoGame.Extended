using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame.Extended.Graphics
{
    [DebuggerDisplay("{Index} {Duration}")]
    public class SpriteSheetAnimationFrame
    {
        public int FrameIndex { get; }
        public TimeSpan Duration { get; }

        public Texture2DRegion TextureRegion { get; }

        public SpriteSheetAnimationFrame(int index, Texture2DRegion region, TimeSpan duration)
        {
            ArgumentNullException.ThrowIfNull(region);
            if (region.Texture.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(region), $"The source {nameof(Texture2D)} of {nameof(region)} was disposed prior.");
            }

            FrameIndex = index;
            Duration = duration;
        }
    }
}
