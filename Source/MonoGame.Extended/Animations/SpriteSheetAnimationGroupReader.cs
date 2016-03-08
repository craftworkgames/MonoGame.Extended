using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimationGroupReader : ContentTypeReader<SpriteSheetAnimationGroup>
    {
        protected override SpriteSheetAnimationGroup Read(ContentReader reader, SpriteSheetAnimationGroup existingInstance)
        {
            var textureAtlasAssetName = reader.GetRelativeAssetPath(reader.ReadString());
            var textureAtlas = reader.ContentManager.Load<TextureAtlas>(textureAtlasAssetName);
            var frameCount = reader.ReadInt32();
            var regions = new List<TextureRegion2D>();
            var animations = new List<SpriteSheetAnimation>();

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

                var frameIndexCount = reader.ReadInt32();
                var frameIndices = new int[frameIndexCount];

                for (var f = 0; f < frameIndexCount; f++)
                {
                    frameIndices[f] = reader.ReadInt32();
                }

                animations.Add(new SpriteSheetAnimation(name, framesPerSecond, frameIndices));
            }

            return new SpriteSheetAnimationGroup(regions, animations);
        }
    }
}