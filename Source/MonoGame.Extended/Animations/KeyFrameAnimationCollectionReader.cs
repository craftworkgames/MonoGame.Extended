using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class KeyFrameAnimationCollectionReader : ContentTypeReader<KeyFrameAnimationCollection>
    {
        protected override KeyFrameAnimationCollection Read(ContentReader reader, KeyFrameAnimationCollection existingInstance)
        {
            var textureAtlasAssetName = reader.GetRelativeAssetPath(reader.ReadString());
            var textureAtlas = reader.ContentManager.Load<TextureAtlas>(textureAtlasAssetName);
            var frameCount = reader.ReadInt32();
            var regions = new List<TextureRegion2D>();
            var animations = new KeyFrameAnimationCollection();

            for (var i = 0; i < frameCount; i++)
            {
                var frameName = reader.ReadString();
                var textureRegion = textureAtlas[frameName];
                regions.Add(textureRegion);
            }

            var animationCount = reader.ReadInt32();

            for (var i = 0; i < animationCount; i++)
            {
                var name = reader.ReadString();
                var framesPerSecond = reader.ReadInt32();
                var isLooping = reader.ReadBoolean();
                var isReversed = reader.ReadBoolean();
                var isPingPong = reader.ReadBoolean();
                var frameDuration = 1.0f / framesPerSecond;
                var frameIndexCount = reader.ReadInt32();
                var keyFrames = new TextureRegion2D[frameIndexCount];

                for (var f = 0; f < frameIndexCount; f++)
                {
                    var frameIndex = reader.ReadInt32();
                    keyFrames[f] = regions[frameIndex];
                }
                
                animations.Add(new KeyFrameAnimation(name, frameDuration, keyFrames, isLooping, isReversed, isPingPong));
            }

            return animations;
        }
    }
}
