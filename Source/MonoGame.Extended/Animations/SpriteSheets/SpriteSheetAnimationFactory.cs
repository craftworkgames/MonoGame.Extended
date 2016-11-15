using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimationFactory
    {
        private readonly Dictionary<string, SpriteSheetAnimationData> _animationDataDictionary;

        public SpriteSheetAnimationFactory(TextureAtlas textureAtlas)
            : this(textureAtlas.Regions)
        {
        }

        public SpriteSheetAnimationFactory(IEnumerable<TextureRegion2D> frames)
        {
            _animationDataDictionary = new Dictionary<string, SpriteSheetAnimationData>();
            Frames = frames.ToArray();
        }

        public IReadOnlyList<TextureRegion2D> Frames { get; }

        public void Add(string name, SpriteSheetAnimationData data)
        {
            _animationDataDictionary.Add(name, data);
        }

        public void Remove(string name)
        {
            _animationDataDictionary.Remove(name);
        }

        public SpriteSheetAnimation Create(string name)
        {
            SpriteSheetAnimationData data;

            if (_animationDataDictionary.TryGetValue(name, out data))
            {
                var keyFrames = data.FrameIndicies
                    .Select(i => Frames[i])
                    .ToArray();

                return new SpriteSheetAnimation(name, keyFrames, data);
            }

            return null;
        }
    }
}