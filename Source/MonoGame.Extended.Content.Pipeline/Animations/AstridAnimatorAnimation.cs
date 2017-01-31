using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class AstridAnimatorAnimation
    {
        public string Name { get; set; }
        public int FramesPerSecond { get; set; }
        public List<string> Frames { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }

        public AstridAnimatorAnimation(string name, int framesPerSecond)
        {
            Name = name;
            FramesPerSecond = framesPerSecond;
            Frames = new List<string>();
            IsLooping = true;
            IsReversed = false;
            IsPingPong = false;
        }
    }
}