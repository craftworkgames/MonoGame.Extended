using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class AstridAnimatorProcessorResult
    {
        public AstridAnimatorProcessorResult(AstridAnimatorFile data, IEnumerable<string> frames)
        {
            Data = data;
            Frames = new List<string>(frames);
        }

        public AstridAnimatorFile Data { get; private set; }
        public List<string> Frames { get; private set; } 
    }
}