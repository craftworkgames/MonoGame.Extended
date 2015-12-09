using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class AstridAnimatorReader : ContentTypeReader<SpriteSheetAnimator>
    {
        protected override SpriteSheetAnimator Read(ContentReader reader, SpriteSheetAnimator existingInstance)
        {
            var textureAtlasAssetName = reader.GetRelativeAssetPath(reader.ReadString());
            var textureAtlas = reader.ContentManager.Load<TextureAtlas>(textureAtlasAssetName);
            var sprite = new Sprite(textureAtlas.First());
            var animator = new SpriteSheetAnimator(sprite, textureAtlas);
            var frameCount = reader.ReadInt32();

            for (var i = 0; i < frameCount; i++)
            {
                var frameName = reader.ReadString();
                var textureRegion = textureAtlas[frameName];
                animator.AddFrame(textureRegion);
            }

            var animationCount = reader.ReadInt32();

            for (var i = 0; i < animationCount; i++)
            {
                var name = reader.ReadString();
                var framesPerSecond = reader.ReadInt32();

                var frameIndexCount = reader.ReadInt32();
                var frameIndices = new int[frameIndexCount];

                for (var f = 0; f < frameIndexCount; f++)
                    frameIndices[f] = reader.ReadInt32();

                animator.AddAnimation(name, framesPerSecond, frameIndices);
            }

            return animator;
        }
    }
}
