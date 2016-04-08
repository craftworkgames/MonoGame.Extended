using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimationGroup
    {
        internal SpriteSheetAnimationGroup(IEnumerable<TextureRegion2D> frames, IEnumerable<SpriteSheetAnimation> animations)
        {
            _frames = frames.ToArray();
            _animations = animations.ToDictionary(a => a.Name);
        }
        
        private readonly Dictionary<string, SpriteSheetAnimation> _animations;
        public IEnumerable<string> Animations
        {
            get { return _animations.Keys.OrderBy(i => i); }
        }

        private readonly TextureRegion2D[] _frames;
        public IEnumerable<TextureRegion2D> Frames => _frames;

        public SpriteSheetAnimation GetAnimation(string name)
        {
            return _animations[name];
        }

        public TextureRegion2D GetFrame(int index)
        {
            return _frames[index];
        }

        public SpriteSheetAnimation this[string name] => GetAnimation(name);

        public bool Contains(SpriteSheetAnimation animation)
        {
            return _animations.ContainsValue(animation);
        }
    }
}