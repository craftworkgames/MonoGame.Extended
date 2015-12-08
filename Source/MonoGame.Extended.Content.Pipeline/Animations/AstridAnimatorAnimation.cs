using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class AstridAnimatorAnimation
    {
        public AstridAnimatorAnimation(string name, int framesPerSecond)
        {
            Name = name;
            FramesPerSecond = framesPerSecond;
            Frames = new List<string>();
        }

        public string Name { get; set; }
        public int FramesPerSecond { get; set; }
        public List<string> Frames { get; set; }
    }
}