using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class SpriteSheet
    {
        public SpriteSheet()
        {
            Cycles = new Dictionary<string, SpriteSheetAnimationCycle>();
        }

        public TextureAtlas TextureAtlas { get; set; }
        public Dictionary<string, SpriteSheetAnimationCycle> Cycles { get; set; }

        public SpriteSheetAnimation CreateAnimation(string name)
        {
            var cycle = Cycles[name];
            var keyFrames = cycle.Frames
                .Select(f => TextureAtlas[f.Index])
                .ToArray();

            return new SpriteSheetAnimation(name, keyFrames, cycle.FrameDuration, cycle.IsLooping, cycle.IsReversed, cycle.IsPingPong);
        }
    }
}