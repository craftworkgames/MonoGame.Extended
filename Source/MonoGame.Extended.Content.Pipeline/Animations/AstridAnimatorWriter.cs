using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentTypeWriter]
    public class AstridAnimatorWriter : ContentTypeWriter<AstridAnimatorProcessorResult>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof (SpriteSheetAnimationGroupReader).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter writer, AstridAnimatorProcessorResult input)
        {
            var data = input.Data;

            writer.Write(input.TextureAtlasAssetName);
            writer.Write(input.Frames.Count);

            foreach (var frame in input.Frames)
            {
                writer.Write(frame);
            }

            writer.Write(data.Animations.Count);

            foreach (var animation in data.Animations)
            {
                writer.Write(animation.Name);
                writer.Write(animation.FramesPerSecond);
                writer.Write(animation.Frames.Count);

                foreach (var frame in animation.Frames)
                {
                    writer.Write(input.Frames.IndexOf(frame));
                }
            }
        }
    }
}