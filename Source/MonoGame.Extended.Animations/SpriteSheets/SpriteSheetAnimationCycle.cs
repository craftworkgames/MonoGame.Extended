using System.Collections.Generic;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimationCycle
    {
        public float FrameDuration { get; set; } = 0.2f;
        public List<SpriteSheetAnimationFrame> Frames { get; set; }
    }
}