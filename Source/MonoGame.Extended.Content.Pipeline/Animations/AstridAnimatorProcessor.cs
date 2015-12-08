using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.TextureAtlases;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentProcessor(DisplayName = "Astrid Animator Processor - MonoGame.Extended")]
    public class AstridAnimatorProcessor : ContentProcessor<AstridAnimatorFile, AstridAnimatorProcessorResult>
    {
        public override AstridAnimatorProcessorResult Process(AstridAnimatorFile input, ContentProcessorContext context)
        {
            var directory = Path.GetDirectoryName(input.TextureAtlas); // TODO
            var atlasPath = Path.Combine("TestData", input.TextureAtlas);
            var atlasImporter = new TexturePackerJsonImporter();
            var atlas = atlasImporter.Import(atlasPath, null);
            

            return new AstridAnimatorProcessorResult();
        }
    }
}