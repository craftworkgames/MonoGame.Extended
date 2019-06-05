using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheet
    {
        public SpriteSheet()
        {
            Cycles = new List<SpriteSheetAnimationCycle>();
        }

        public TextureAtlas TextureAtlas { get; set; }
        public List<SpriteSheetAnimationCycle> Cycles { get; }
    }

    public class SpriteSheetAnimationCycle
    {
        public string Name { get; set; }
        public float DefaultFrameDuration { get; set; } = 0.2f;
        public List<SpriteSheetAnimationFrame> Frames { get; set; }
    }

    public class SpriteSheetAnimationFrame
    {
        public int Index { get; set; }
        public float Duration { get; set; }
    }

    public class SpriteSheetAnimationData
    {
        public SpriteSheetAnimationData(int[] frameIndicies, float frameDuration = 0.2f, bool isLooping = true,
            bool isReversed = false, bool isPingPong = false)
        {
            FrameIndicies = frameIndicies;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            IsReversed = isReversed;
            IsPingPong = isPingPong;
        }

        public int[] FrameIndicies { get; }
        public float FrameDuration { get; }
        public bool IsLooping { get; }
        public bool IsReversed { get; }
        public bool IsPingPong { get; }
    }
}