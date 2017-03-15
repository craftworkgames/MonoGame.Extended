using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentProcessor(DisplayName = "Astrid Animator Processor - MonoGame.Extended")]
    public class AstridAnimatorProcessor :
        ContentProcessor<ContentImporterResult<AstridAnimatorFile>, AstridAnimatorProcessorResult>
    {
        public override AstridAnimatorProcessorResult Process(ContentImporterResult<AstridAnimatorFile> input,
            ContentProcessorContext context)
        {
            var data = input.Data;
            var directory = Path.GetDirectoryName(input.FilePath);
            var frames = data.Animations
                .SelectMany(i => i.Frames)
                .OrderBy(f => f)
                .Distinct();

            return new AstridAnimatorProcessorResult(directory, data, frames);
        }
    }
}