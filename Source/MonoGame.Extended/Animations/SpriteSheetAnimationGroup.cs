using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimationGroup
    {
        private readonly Dictionary<string, SpriteSheetAnimation> _animations;

        private readonly TextureRegion2D[] _frames;

        public IEnumerable<string> Animations
        {
            get { return _animations.Keys.OrderBy(i => i); }
        }

        public IEnumerable<TextureRegion2D> Frames => _frames;

        public SpriteSheetAnimation this[string name] => GetAnimation(name);

        internal SpriteSheetAnimationGroup(IEnumerable<TextureRegion2D> frames, IEnumerable<SpriteSheetAnimation> animations)
        {
            _frames = frames.ToArray();
            _animations = animations.ToDictionary(a => a.Name);
        }

        public SpriteSheetAnimation GetAnimation(string name)
        {
            return _animations[name];
        }

        public TextureRegion2D GetFrame(int index)
        {
            return _frames[index];
        }

        public bool Contains(SpriteSheetAnimation animation)
        {
            return _animations.ContainsValue(animation);
        }
    }
}