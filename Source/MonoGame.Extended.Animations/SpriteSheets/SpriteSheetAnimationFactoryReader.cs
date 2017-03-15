using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimationFactoryReader : ContentTypeReader<SpriteSheetAnimationFactory>
    {
        protected override SpriteSheetAnimationFactory Read(ContentReader reader, SpriteSheetAnimationFactory existingInstance)
        {
            var textureAtlasAssetName = reader.GetRelativeAssetName(reader.ReadString());
            var textureAtlas = reader.ContentManager.Load<TextureAtlas>(textureAtlasAssetName);
            var frameCount = reader.ReadInt32();
            var regions = new List<TextureRegion2D>();

            for (var i = 0; i < frameCount; i++)
            {
                var frameName = reader.ReadString();
                var textureRegion = textureAtlas[frameName];
                regions.Add(textureRegion);
            }

            var animationFactory = new SpriteSheetAnimationFactory(regions);
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
                var frameIndicies = new int[frameIndexCount];

                for (var f = 0; f < frameIndexCount; f++)
                {
                    var frameIndex = reader.ReadInt32();
                    frameIndicies[f] = frameIndex;
                }

                var animationData = new SpriteSheetAnimationData(frameIndicies, frameDuration, isLooping, isReversed,
                    isPingPong);
                animationFactory.Add(name, animationData);
            }

            return animationFactory;
        }
    }
}