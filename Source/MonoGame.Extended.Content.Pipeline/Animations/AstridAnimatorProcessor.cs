using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.TextureAtlases;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentProcessor(DisplayName = "Astrid Animator Processor - MonoGame.Extended")]
    public class AstridAnimatorProcessor : ContentProcessor<AstridAnimatorFile, AstridAnimatorProcessorResult>
    {
        public override AstridAnimatorProcessorResult Process(AstridAnimatorFile input, ContentProcessorContext context)
        {
            //var data = input.Data;
            //var directory = Path.GetDirectoryName(input.FilePath);
            //Debug.Assert(directory != null, "directory != null");
            //var atlasPath = Path.Combine(directory, data.TextureAtlas);
            //var atlasImporter = new TexturePackerJsonImporter();
            //var atlas = atlasImporter.Import(atlasPath, null);

            var frames = input.Animations
                .SelectMany(i => i.Frames)
                .OrderBy(f => f)
                .Distinct();
            var result = new AstridAnimatorProcessorResult(input, frames);
            return result;
        }
    }
}